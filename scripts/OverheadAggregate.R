# =========================
# OverheadAggregate.R
# Aggregates BenchmarkDotNet overhead + correctness results
# Folder layout: data/run01_Energy, data/run01_NoEnergy, ...
# =========================

suppressPackageStartupMessages({
  library(dplyr)
  library(stringr)
  library(tibble)
  library(readr)
  library(tidyr)
  library(ggplot2)
  library(knitr)
  library(purrr)
})

# -------------------------
# Paths (robust)
# -------------------------
args <- commandArgs(trailingOnly = FALSE)
file_arg <- args[grep("^--file=", args)]
if (length(file_arg) == 0) stop("Run with: Rscript scripts/OverheadAggregate.R")

script_path <- sub("^--file=", "", file_arg[1])
script_dir <- dirname(normalizePath(script_path))
project_root <- normalizePath(file.path(script_dir, ".."))
base_dir <- file.path(project_root, "data")

cat("script_dir:", script_dir, "\n")
cat("project_root:", project_root, "\n")
cat("base_dir:", base_dir, "\n")

# -------------------------
# Config
# -------------------------
folder_regex <- "^run(\\d+)_(Energy|NoEnergy)$"
report_regex <- "(?i)report.*\\.csv$"
meas_regex   <- "(?i)measurements.*\\.csv$"
log_regex    <- "\\.log$"

# -------------------------
# Helpers
# -------------------------
first_match <- function(dir, pattern) {
  x <- list.files(dir, full.names = TRUE)
  x <- x[str_detect(basename(x), pattern)]
  if (length(x) == 0) return(NA_character_)
  x[1]
}

parse_num <- function(x) as.numeric(gsub(",", "", x))

parse_time_to_ns <- function(x) {
  s <- trimws(gsub("\"", "", x))
  parts <- strsplit(s, "\\s+")[[1]]
  if (length(parts) < 2) return(NA_real_)

  val <- as.numeric(gsub(",", "", parts[1]))
  unit <- parts[2]
  if (is.na(val)) return(NA_real_)

  if (unit == "ns") return(val)
  if (unit == "us") return(val * 1e3)
  if (unit == "ms") return(val * 1e6)
  if (unit == "s")  return(val * 1e9)
  NA_real_
}

read_one_report <- function(path, run_id, condition) {
  df <- read_csv(path, show_col_types = FALSE)
  df %>%
    transmute(
      run_id = run_id,
      condition = condition,
      method = .data$Method,
      job = .data$Job,
      mean_ns = vapply(.data$Mean, parse_time_to_ns, numeric(1)),
      sd_ns   = vapply(.data$StdDev, parse_time_to_ns, numeric(1)),
      operations = parse_num(.data$Operations),
      iterations = as.numeric(.data$Iterations)
    )
}

parse_log_times <- function(log_path) {
  lines <- readLines(log_path, warn = FALSE)

  run_line  <- grep("^Run time:", lines, value = TRUE)
  glob_line <- grep("^Global total time:", lines, value = TRUE)

  run_sec  <- as.numeric(str_match(run_line,  "\\(([^ ]+) sec\\)")[,2])
  glob_sec <- as.numeric(str_match(glob_line, "\\(([^ ]+) sec\\)")[,2])

  tibble(run_time_sec = run_sec, global_time_sec = glob_sec)
}

# Stratified bootstrap CI for overhead:
# overhead% = 100 * (mean(Energy)/mean(NoEnergy) - 1)
strat_boot_overhead_ci <- function(df, value_col,
                                   conf_levels = c(0.95, 0.99, 0.9999),
                                   B = 20000,
                                   seed = 1) {
  set.seed(seed)

  value_name <- deparse(substitute(value_col))

  A <- df %>% filter(condition=="NoEnergy") %>% pull(.data[[value_name]])
  E <- df %>% filter(condition=="Energy") %>% pull(.data[[value_name]])

  # remove NA just in case (should already be clean if you used the check)
  A <- A[!is.na(A)]
  E <- E[!is.na(E)]

  if (length(A) < 2 || length(E) < 2) {
    stop("Not enough samples per condition to compute CI.")
  }

  nA <- length(A)
  nE <- length(E)

  # point estimate
  point <- 100 * (mean(E) / mean(A) - 1)

  # stratified bootstrap: resample within each condition, keep same counts
  boot <- replicate(B, {
    A_b <- sample(A, nA, replace = TRUE)
    E_b <- sample(E, nE, replace = TRUE)
    100 * (mean(E_b) / mean(A_b) - 1)
  })

  # compute CIs
  out <- lapply(conf_levels, function(cl) {
    alpha <- (1 - cl) / 2
    tibble(
      conf_level = cl,
      overhead_pct = point,
      ci_low = as.numeric(quantile(boot, alpha, na.rm = TRUE)),
      ci_high = as.numeric(quantile(boot, 1 - alpha, na.rm = TRUE))
    )
  }) %>% bind_rows()

  out
}

fmt_ci <- function(low, high, digits = 2) {
  sprintf(paste0("[%.", digits, "f, %.", digits, "f]"), low, high)
}

conf_label <- function(cl) {
  if (abs(cl - 0.95) < 1e-12) return("95")
  if (abs(cl - 0.99) < 1e-12) return("99")
  if (abs(cl - 0.9999) < 1e-12) return("99.99")
  as.character(cl)
}

# -------------------------
# 1) Discover run folders + file paths
# -------------------------
run_dirs <- list.dirs(base_dir, recursive = FALSE, full.names = TRUE)

runs_files <- tibble(dir = run_dirs) %>%
  mutate(folder = basename(dir)) %>%
  filter(str_detect(folder, folder_regex)) %>%
  mutate(
    run_id = as.integer(str_match(folder, folder_regex)[,2]),
    condition = str_match(folder, folder_regex)[,3],
    report_path = map_chr(dir, first_match, pattern = report_regex),
    meas_path   = map_chr(dir, first_match, pattern = meas_regex),
    log_path    = map_chr(dir, first_match, pattern = log_regex)
  ) %>%
  arrange(run_id, condition)

print(runs_files)

missing <- runs_files %>%
  filter(is.na(report_path) | is.na(meas_path) | is.na(log_path))

if (nrow(missing) > 0) {
  cat("\nERROR: Some run folders are missing files:\n")
  print(missing)
  stop("Fix missing files before aggregating.")
} else {
  cat("\nOK: All run folders have report + measurements + log.\n")
}

# -------------------------
# 2) Aggregate report.csv (timing correctness data)
# -------------------------
reports_all <- runs_files %>%
  pmap_dfr(function(dir, folder, run_id, condition, report_path, meas_path, log_path) {
    read_one_report(report_path, run_id, condition)
  })

write_csv(reports_all, file.path(base_dir, "agg_reports_raw.csv"))
cat("Wrote:", file.path(base_dir, "agg_reports_raw.csv"), "\n")

# -------------------------
# 3) Aggregate runtimes from logs (overhead data)
# -------------------------

runtimes_all <- runs_files %>%
  rowwise() %>%
  mutate(times = list(parse_log_times(log_path))) %>%
  unnest(times) %>%
  ungroup() %>%
  select(run_id, condition, run_time_sec, global_time_sec, log_path)

bad_times <- runtimes_all %>%
  filter(is.na(run_time_sec) | is.na(global_time_sec))

if (nrow(bad_times) > 0) {
  cat("\nERROR: Some logs did not yield runtime seconds (NA). Check these:\n")
  print(bad_times)
  stop("Fix log parsing before CI computation.")
}

write_csv(runtimes_all, file.path(base_dir, "agg_runtimes_raw.csv"))
cat("Wrote:", file.path(base_dir, "agg_runtimes_raw.csv"), "\n")

# -------------------------
# 3b) Diagnostics: histogram+density + Q-Q plots
# -------------------------
runtime_diag <- runtimes_all %>%
  select(condition, run_time_sec, global_time_sec) %>%
  pivot_longer(
    cols = c(run_time_sec, global_time_sec),
    names_to = "metric",
    values_to = "seconds"
  ) %>%
  mutate(metric = recode(metric,
                         run_time_sec = "Benchmark run time",
                         global_time_sec = "Global total time"))

# Histogram + density
p_hist <- ggplot(runtime_diag, aes(x = seconds)) +
  geom_histogram(aes(y = after_stat(density)), bins = 10) +
  geom_density() +
  facet_grid(metric ~ condition, scales = "free_x") +
  labs(
    title = "Runtime distributions (histogram + density)",
    x = "Seconds",
    y = "Density"
  )

ggsave(file.path(base_dir, "diag_runtime_hist_density.png"),
       plot = p_hist, width = 10, height = 5)
cat("Wrote:", file.path(base_dir, "diag_runtime_hist_density.png"), "\n")

# Q-Q plot
p_qq <- ggplot(runtime_diag, aes(sample = seconds)) +
  stat_qq() +
  stat_qq_line() +
  facet_grid(metric ~ condition, scales = "free") +
  labs(
    title = "Runtime normal Q–Q plots",
    x = "Theoretical quantiles",
    y = "Sample quantiles"
  )

ggsave(file.path(base_dir, "diag_runtime_qq.png"),
       plot = p_qq, width = 10, height = 5)
cat("Wrote:", file.path(base_dir, "diag_runtime_qq.png"), "\n")

# Figure: distribution of runtimes by condition (boxplot + points)
runtime_dist <- runtimes_all %>%
  select(condition, run_time_sec, global_time_sec) %>%
  pivot_longer(cols = c(run_time_sec, global_time_sec),
               names_to = "metric",
               values_to = "seconds") %>%
  mutate(metric = recode(metric,
                         run_time_sec = "Benchmark run time",
                         global_time_sec = "Global total time"))

p_dist <- ggplot(runtime_dist, aes(x = condition, y = seconds)) +
  geom_boxplot(outlier.shape = NA) +
  geom_jitter(width = 0.12, height = 0, alpha = 0.7) +
  facet_wrap(~ metric, ncol = 2, scales = "free_y") +
  labs(
    title = "Runtime distribution by condition",
    x = NULL,
    y = "Seconds"
  )

ggsave(file.path(base_dir, "fig_runtime_distribution.png"),
       plot = p_dist, width = 9, height = 4)

# -------------------------
# 4) Runtime summary + per-run deltas
# -------------------------
runtime_summary <- runtimes_all %>%
  group_by(condition) %>%
  summarise(
    n = n(),
    run_time_mean = mean(run_time_sec),
    run_time_sd   = sd(run_time_sec),
    run_time_se   = run_time_sd / sqrt(n),
    global_time_mean = mean(global_time_sec),
    global_time_sd   = sd(global_time_sec),
    global_time_se   = global_time_sd / sqrt(n),
    .groups = "drop"
  )

run_overhead <- (runtime_summary$run_time_mean[runtime_summary$condition=="Energy"] /
                 runtime_summary$run_time_mean[runtime_summary$condition=="NoEnergy"] - 1) * 100

global_overhead <- (runtime_summary$global_time_mean[runtime_summary$condition=="Energy"] /
                    runtime_summary$global_time_mean[runtime_summary$condition=="NoEnergy"] - 1) * 100

runtime_summary <- runtime_summary %>%
  mutate(
    run_overhead_pct = if_else(condition=="Energy", run_overhead, -run_overhead),
    global_overhead_pct = if_else(condition=="Energy", global_overhead, -global_overhead)
  ) %>%
  mutate(condition = factor(condition, levels = c("Energy","NoEnergy"))) %>%
  arrange(condition) %>%
  mutate(condition = as.character(condition))

# --- Add CIs to runtime_summary (95, 99, 99.99) ---
ci_levels <- c(0.95, 0.99, 0.9999)
ci_digits <- 2

ci_run <- strat_boot_overhead_ci(runtimes_all, run_time_sec,
                                 conf_levels = ci_levels, B = 20000, seed = 1) %>%
  mutate(level = vapply(conf_level, conf_label, character(1)),
         ci_str = fmt_ci(ci_low, ci_high, digits = ci_digits)) %>%
  select(level, ci_str)

ci_global <- strat_boot_overhead_ci(runtimes_all, global_time_sec,
                                    conf_levels = ci_levels, B = 20000, seed = 1) %>%
  mutate(level = vapply(conf_level, conf_label, character(1)),
         ci_str = fmt_ci(ci_low, ci_high, digits = ci_digits)) %>%
  select(level, ci_str)

run_ci_map <- setNames(ci_run$ci_str, ci_run$level)
global_ci_map <- setNames(ci_global$ci_str, ci_global$level)

runtime_summary <- runtime_summary %>%
  mutate(
    `Run OH 95% CI`     = run_ci_map[["95"]],
    `Run OH 99% CI`     = run_ci_map[["99"]],
    `Run OH 99.99% CI`  = run_ci_map[["99.99"]],
    `Global OH 95% CI`    = global_ci_map[["95"]],
    `Global OH 99% CI`    = global_ci_map[["99"]],
    `Global OH 99.99% CI` = global_ci_map[["99.99"]]
  )

write_csv(runtime_summary, file.path(base_dir, "runtime_summary.csv"))
cat("Wrote:", file.path(base_dir, "runtime_summary.csv"), "\n")

# -------------------------
# 5) Correctness table (Energy vs NoEnergy mean ns)
# -------------------------
means_by_condition <- reports_all %>%
  group_by(condition, method) %>%
  summarise(mean_ns = mean(mean_ns), .groups="drop")

correctness_overall <- means_by_condition %>%
  pivot_wider(names_from = condition, values_from = mean_ns) %>%
  rename(
    mean_ns_energy = Energy,
    mean_ns_noenergy = NoEnergy
  ) %>%
  mutate(
    ratio = mean_ns_energy / mean_ns_noenergy,
    delta_pct = (ratio - 1) * 100
  ) %>%
  arrange(desc(abs(delta_pct)))

write_csv(correctness_overall, file.path(base_dir, "correctness_overall.csv"))
cat("Wrote:", file.path(base_dir, "correctness_overall.csv"), "\n")

correctness_counts <- correctness_overall %>%
  summarise(
    within_1pct = sum(abs(delta_pct) <= 1),
    within_2pct = sum(abs(delta_pct) <= 2),
    within_5pct = sum(abs(delta_pct) <= 5),
    total = n()
  )

write_csv(correctness_counts, file.path(base_dir, "correctness_counts.csv"))
cat("Wrote:", file.path(base_dir, "correctness_counts.csv"), "\n")

# -------------------------
# 6) Figures
# -------------------------
# Figure 2: correctness delta % per benchmark
correctness_plot_data <- correctness_overall %>%
  mutate(method = reorder(method, abs(delta_pct)))

p_correct <- ggplot(correctness_plot_data, aes(x = method, y = delta_pct)) +
  annotate("rect", xmin=-Inf, xmax=Inf, ymin=-2, ymax=2, alpha=0.1) +
  geom_hline(yintercept = 0) +
  geom_point() +
  coord_flip() +
  labs(
    title = "Measurement correctness: timing difference with energy diagnoser enabled",
    x = "Benchmark method",
    y = "Δ time (%) (Energy vs NoEnergy)"
  )

ggsave(file.path(base_dir, "fig_correctness_delta_pct.png"),
       plot = p_correct, width = 10, height = 8)
cat("Wrote:", file.path(base_dir, "fig_correctness_delta_pct.png"), "\n")

# -------------------------
# 7) LaTeX table for Overleaf (runtime summary)
# -------------------------
sec_digits <- 1
pct_digits <- 2
fmt_pct <- function(x) sprintf(paste0("%+.", pct_digits, "f"), x)

# --- Table A: Benchmark runtime ---
runtime_table_bench <- runtime_summary %>%
  transmute(
    Condition = condition,
    `Benchmark (s)` = round(run_time_mean, sec_digits),
    `SD (s)`        = round(run_time_sd, sec_digits),
    `Overhead (\\%)` = fmt_pct(run_overhead_pct),
    `95\\% CI (\\%)` = .data$`Run OH 95% CI`
  )

latex_bench <- kable(
  runtime_table_bench,
  format = "latex",
  booktabs = TRUE,
  escape = FALSE,
  caption = "Benchmark session runtime with and without energy measurement (mean and SD across runs).",
  label = "runtime-benchmark",
  align = "lrrrr"
)

writeLines(latex_bench, file.path(base_dir, "runtime_benchmark.tex"))
cat("Wrote:", file.path(base_dir, "runtime_benchmark.tex"), "\n")


# --- Table B: Global runtime ---
runtime_table_global <- runtime_summary %>%
  transmute(
    Condition = condition,
    `Global (s)` = round(global_time_mean, sec_digits),
    `SD (s)`     = round(global_time_sd, sec_digits),
    `Overhead (\\%)` = fmt_pct(global_overhead_pct),
    `95\\% CI (\\%)` = .data$`Global OH 95% CI`
  )

latex_global <- kable(
  runtime_table_global,
  format = "latex",
  booktabs = TRUE,
  escape = FALSE,
  caption = "Global total runtime with and without energy measurement (mean and SD across runs).",
  label = "runtime-global",
  align = "lrrrr"
)

writeLines(latex_global, file.path(base_dir, "runtime_global.tex"))
cat("Wrote:", file.path(base_dir, "runtime_global.tex"), "\n")
