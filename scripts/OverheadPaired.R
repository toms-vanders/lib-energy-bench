# =========================
# OverheadAggregate.R (PAIRED t-CI)
# Aggregates BenchmarkDotNet runtime overhead results
# Folder layout: data/run01_Energy, data/run01_NoEnergy, ...
# Energy = Enabled, NoEnergy = Disabled
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
if (length(file_arg) == 0) stop("Run with: Rscript scripts/OverheadPaired.R")

script_path <- sub("^--file=", "", file_arg[1])
script_dir <- dirname(normalizePath(script_path))
project_root <- normalizePath(file.path(script_dir, ".."))
base_dir <- file.path(project_root, "src/Serialization.Bench/data/overhead")

cat("script_dir:", script_dir, "\n")
cat("project_root:", project_root, "\n")
cat("base_dir:", base_dir, "\n")

# -------------------------
# Config
# -------------------------
folder_regex <- "^run(\\d+)_(Energy|NoEnergy)$"
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

parse_log_times <- function(log_path) {
  lines <- readLines(log_path, warn = FALSE)

  run_line  <- grep("^Run time:", lines, value = TRUE)
  glob_line <- grep("^Global total time:", lines, value = TRUE)

  run_sec  <- as.numeric(str_match(run_line,  "\\(([^ ]+) sec\\)")[,2])
  glob_sec <- as.numeric(str_match(glob_line, "\\(([^ ]+) sec\\)")[,2])

  tibble(run_time_sec = run_sec, global_time_sec = glob_sec)
}

fmt_ci <- function(low, high, digits = 2) {
  sprintf(paste0("[%.", digits, "f, %.", digits, "f]"), low, high)
}

fmt_pct <- function(x, digits = 2) sprintf(paste0("%+.", digits, "f"), x)

paired_t_ci <- function(x, conf = 0.95) {
  x <- x[!is.na(x)]
  n <- length(x)
  if (n < 2) stop("Need at least 2 paired differences for CI.")

  m <- mean(x)
  s <- sd(x)
  se <- s / sqrt(n)
  alpha <- 1 - conf
  t_star <- qt(1 - alpha/2, df = n - 1)

  tibble(
    n = n,
    mean = m,
    sd = s,
    se = se,
    t_star = t_star,
    ci_low = m - t_star * se,
    ci_high = m + t_star * se
  )
}

# -------------------------
# 1) Discover run folders + log paths
# -------------------------
run_dirs <- list.dirs(base_dir, recursive = FALSE, full.names = TRUE)

runs_files <- tibble(dir = run_dirs) %>%
  mutate(folder = basename(dir)) %>%
  filter(str_detect(folder, folder_regex)) %>%
  mutate(
    run_id = as.integer(str_match(folder, folder_regex)[,2]),
    condition = str_match(folder, folder_regex)[,3],
    log_path = map_chr(dir, first_match, pattern = log_regex)
  ) %>%
  arrange(run_id, condition)

print(runs_files)

missing <- runs_files %>% filter(is.na(log_path))
if (nrow(missing) > 0) {
  cat("\nERROR: Some run folders are missing log files:\n")
  print(missing)
  stop("Fix missing logs before aggregating.")
} else {
  cat("\nOK: All run folders have log files.\n")
}

# -------------------------
# 2) Aggregate runtimes from logs
# -------------------------
runtimes_all <- runs_files %>%
  rowwise() %>%
  mutate(times = list(parse_log_times(log_path))) %>%
  unnest(times) %>%
  ungroup() %>%
  mutate(
    EnergyDiagnoser = if_else(condition == "Energy", "Enabled", "Disabled"),
    EnergyDiagnoser = factor(EnergyDiagnoser, levels = c("Disabled", "Enabled"))
  ) %>%
  select(run_id, condition, EnergyDiagnoser, run_time_sec, global_time_sec, log_path)

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
# 3) Pair by run_id and compute differences
# -------------------------
paired <- runtimes_all %>%
  select(run_id, EnergyDiagnoser, run_time_sec, global_time_sec) %>%
  pivot_wider(
    names_from = EnergyDiagnoser,
    values_from = c(run_time_sec, global_time_sec)
  ) %>%
  mutate(
    diff_run_time_sec = run_time_sec_Enabled - run_time_sec_Disabled,
    diff_global_time_sec = global_time_sec_Enabled - global_time_sec_Disabled
  )

# sanity: require complete pairs
if (any(is.na(paired$run_time_sec_Disabled) | is.na(paired$run_time_sec_Enabled))) {
  stop("Missing Enabled/Disabled pair for at least one run_id. Check folders/logs.")
}

write_csv(paired, file.path(base_dir, "paired_runtimes.csv"))
cat("Wrote:", file.path(base_dir, "paired_runtimes.csv"), "\n")

# -------------------------
# Diagnostics: Q–Q plots of paired differences (STACKED)
# -------------------------
diff_long <- paired %>%
  select(run_id, diff_run_time_sec, diff_global_time_sec) %>%
  pivot_longer(
    cols = c(diff_run_time_sec, diff_global_time_sec),
    names_to = "metric",
    values_to = "diff_sec"
  ) %>%
  mutate(metric = recode(metric,
                         diff_run_time_sec       = "Run Time (Enabled − Disabled)",
                         diff_global_time_sec    = "Global Total Time (Enabled − Disabled)"))

p_qq_diff <- ggplot(diff_long, aes(sample = diff_sec)) +
  stat_qq() +
  stat_qq_line() +
  facet_wrap(~ metric, ncol = 1, scales = "free") +
  labs(
    title = "Q–Q plots of paired runtime differences",
    x = "Theoretical quantiles",
    y = "Sample quantiles (seconds)"
  )

ggsave(file.path(base_dir, "diag_qq_paired_differences_stacked.png"),
       plot = p_qq_diff, width = 7, height = 7, dpi = 300)

cat("Wrote:", file.path(base_dir, "diag_qq_paired_differences_stacked.png"), "\n")

# -------------------------
# 5) Figures: runtime distributions (two separate figures)
# (keeps jitter consistent across figures via deterministic x offsets)
# -------------------------
jwidth <- 0.12

runtime_dist <- runtimes_all %>%
  mutate(
    x_base = as.numeric(EnergyDiagnoser),
    x_jit  = x_base + (((run_id * 37) %% 101) / 101 - 0.5) * 2 * jwidth
  ) %>%
  select(run_id, EnergyDiagnoser, x_base, x_jit, run_time_sec, global_time_sec) %>%
  pivot_longer(
    cols = c(run_time_sec, global_time_sec),
    names_to = "metric",
    values_to = "seconds"
  ) %>%
  mutate(metric = recode(metric,
                         run_time_sec    = "Run Time",
                         global_time_sec = "Global Total Time"))

bench_data <- runtime_dist %>% filter(metric == "Run Time")
glob_data  <- runtime_dist %>% filter(metric == "Global Total Time")

p_bench <- ggplot(bench_data, aes(y = seconds)) +
  geom_boxplot(aes(x = x_base, group = EnergyDiagnoser), outlier.shape = NA) +
  geom_point(aes(x = x_jit), alpha = 0.7) +
  scale_x_continuous(breaks = c(1, 2), labels = c("Disabled", "Enabled")) +
  labs(
    title = "Run Time",
    subtitle = "EnergyDiagnoser disabled vs. enabled",
    x = NULL,
    y = "Seconds"
  )

p_global <- ggplot(glob_data, aes(y = seconds)) +
  geom_boxplot(aes(x = x_base, group = EnergyDiagnoser), outlier.shape = NA) +
  geom_point(aes(x = x_jit), alpha = 0.7) +
  scale_x_continuous(breaks = c(1, 2), labels = c("Disabled", "Enabled")) +
  labs(
    title = "Global Total Time",
    subtitle = "EnergyDiagnoser disabled vs. enabled",
    x = NULL,
    y = "Seconds"
  )

ggsave(file.path(base_dir, "fig_runtime_benchmark_distribution.png"),
       plot = p_bench, width = 7, height = 4, dpi = 300)
ggsave(file.path(base_dir, "fig_runtime_global_distribution.png"),
       plot = p_global, width = 7, height = 4, dpi = 300)

cat("Wrote: fig_runtime_benchmark_distribution.png\n")
cat("Wrote: fig_runtime_global_distribution.png\n")

# -------------------------
# 6) Summary + paired t CIs (seconds) + derived overhead (%)
# -------------------------
# group summaries
summary_by_setting <- runtimes_all %>%
  group_by(EnergyDiagnoser) %>%
  summarise(
    n = n(),
    run_time_mean = mean(run_time_sec),
    run_time_sd   = sd(run_time_sec),
    global_time_mean = mean(global_time_sec),
    global_time_sd   = sd(global_time_sec),
    .groups = "drop"
  ) %>%
  arrange(EnergyDiagnoser)

# paired t CIs on differences
ci_run_sec <- paired_t_ci(paired$diff_run_time_sec, conf = 0.95)
ci_glob_sec <- paired_t_ci(paired$diff_global_time_sec, conf = 0.95)

# baseline means (Disabled)
baseline_run <- summary_by_setting$run_time_mean[summary_by_setting$EnergyDiagnoser == "Disabled"]
baseline_glob <- summary_by_setting$global_time_mean[summary_by_setting$EnergyDiagnoser == "Disabled"]

# point overhead (%) using mean diff / baseline mean
oh_run_pct <- 100 * (ci_run_sec$mean / baseline_run)
oh_glob_pct <- 100 * (ci_glob_sec$mean / baseline_glob)

# convert seconds CI to percent CI using same baseline mean
oh_run_ci_low <- 100 * (ci_run_sec$ci_low / baseline_run)
oh_run_ci_high <- 100 * (ci_run_sec$ci_high / baseline_run)

oh_glob_ci_low <- 100 * (ci_glob_sec$ci_low / baseline_glob)
oh_glob_ci_high <- 100 * (ci_glob_sec$ci_high / baseline_glob)

runtime_summary <- summary_by_setting %>%
  mutate(
    # overhead only meaningful on Enabled row
    run_overhead_pct = if_else(EnergyDiagnoser == "Enabled", oh_run_pct, NA_real_),
    run_overhead_ci  = if_else(EnergyDiagnoser == "Enabled", fmt_ci(oh_run_ci_low, oh_run_ci_high, 2), "--"),
    run_diff_sec     = if_else(EnergyDiagnoser == "Enabled", ci_run_sec$mean, NA_real_),
    run_diff_ci_sec  = if_else(EnergyDiagnoser == "Enabled", fmt_ci(ci_run_sec$ci_low, ci_run_sec$ci_high, 2), "--"),

    global_overhead_pct = if_else(EnergyDiagnoser == "Enabled", oh_glob_pct, NA_real_),
    global_overhead_ci  = if_else(EnergyDiagnoser == "Enabled", fmt_ci(oh_glob_ci_low, oh_glob_ci_high, 2), "--"),
    global_diff_sec     = if_else(EnergyDiagnoser == "Enabled", ci_glob_sec$mean, NA_real_),
    global_diff_ci_sec  = if_else(EnergyDiagnoser == "Enabled", fmt_ci(ci_glob_sec$ci_low, ci_glob_sec$ci_high, 2), "--")
  )

write_csv(runtime_summary, file.path(base_dir, "runtime_summary.csv"))
cat("Wrote:", file.path(base_dir, "runtime_summary.csv"), "\n")

# -------------------------
# 7) LaTeX tables for Overleaf
# -------------------------
sec_digits <- 1
fmt_num <- function(x, digits = 2) sprintf(paste0("%.", digits, "f"), x)

# Benchmark session table
runtime_table_bench <- runtime_summary %>%
  transmute(
    `EnergyDiagnoser`  = as.character(EnergyDiagnoser),
    `Run Time (s)`     = round(run_time_mean, sec_digits),
    `SD (s)`           = round(run_time_sd, sec_digits),
    `Overhead (\\%)`   = if_else(EnergyDiagnoser == "Enabled", fmt_pct(run_overhead_pct, 2), "--"),
    `95\\% CI (\\%)`   = run_overhead_ci
  )
  
latex_bench <- kable(
  runtime_table_bench,
  format = "latex",
  booktabs = TRUE,
  escape = FALSE,
  caption = "Run Time with EnergyDiagnoser disabled vs. enabled (mean and SD across runs). Overhead and 95\\% CI are derived from paired run differences (Enabled $-$ Disabled).",
  label = "runtime-benchmark",
  align = "lrrrr"
)

latex_bench <- sub("\\\\begin\\{table\\}", "\\\\begin{table}[H]", latex_bench)
latex_bench  <- gsub("\\{\\}", "", latex_bench)
writeLines(latex_bench, file.path(base_dir, "runtime_benchmark.tex"))
cat("Wrote:", file.path(base_dir, "runtime_benchmark.tex"), "\n")


# Global total table
runtime_table_global <- runtime_summary %>%
  transmute(
    `EnergyDiagnoser`  = as.character(EnergyDiagnoser),
    `Global Total Time (s)` = round(global_time_mean, sec_digits),
    `SD (s)`           = round(global_time_sd, sec_digits),
    `Overhead (\\%)`   = if_else(EnergyDiagnoser == "Enabled", fmt_pct(global_overhead_pct, 2), "--"),
    `95\\% CI (\\%)`   = global_overhead_ci
  )

latex_global <- kable(
  runtime_table_global,
  format = "latex",
  booktabs = TRUE,
  escape = FALSE,
  caption = "Global Total Time with EnergyDiagnoser disabled vs. enabled (mean and SD across runs). Overhead and 95\\% CI are derived from paired run differences (Enabled $-$ Disabled).",
  label = "runtime-global",
  align = "lrrrr"
)

latex_global <- sub("\\\\begin\\{table\\}", "\\\\begin{table}[H]", latex_global)
latex_global <- gsub("\\{\\}", "", latex_global)
writeLines(latex_global, file.path(base_dir, "runtime_global.tex"))
cat("Wrote:", file.path(base_dir, "runtime_global.tex"), "\n")

cat("\nDone.\n")