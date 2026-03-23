# =========================
# EnergyTimeTables.R
# Builds LaTeX overview tables like the paper example:
# Energy (µJ/op) and Time (µs/op) across Small/Medium/Large
# =========================

suppressPackageStartupMessages({
  library(dplyr)
  library(stringr)
  library(readr)
})

CSV_FILE   <- "../src/Serialization.Bench/BenchmarkArtifacts/results/Serialization.Bench.JsonSerializationBenchmarks-report.csv"
OUTPUT_DIR <- "graphs/analysis"

dir.create(OUTPUT_DIR, showWarnings = FALSE, recursive = TRUE)

# -------------------------
# Parsing helpers
# -------------------------
parse_time_to_us <- function(x) {
  if (is.na(x)) return(NA_real_)
  s <- str_trim(str_replace_all(as.character(x), "\"", ""))

  # Expect: "150,425.1 ns" / "3.12 us" / etc.
  parts <- str_split(s, "\\s+", simplify = TRUE)
  if (ncol(parts) < 2) return(NA_real_)

  val  <- suppressWarnings(as.numeric(str_replace_all(parts[1], ",", "")))
  unit <- parts[2]

  if (is.na(val)) return(NA_real_)

  # Convert to microseconds
  if (unit == "ns") return(val / 1e3)
  if (unit %in% c("us", "µs", "\u00B5s")) return(val)
  if (unit == "ms") return(val * 1e3)
  if (unit == "s")  return(val * 1e6)

  NA_real_
}

parse_energy_uj <- function(x) {
  if (is.na(x)) return(NA_real_)
  s <- tolower(str_trim(as.character(x)))
  s <- str_replace_all(s, ",", "")
  s <- str_replace_all(s, " uj", "")
  suppressWarnings(as.numeric(s))
}

# -------------------------
# Load + reshape
# -------------------------
raw <- read_csv(CSV_FILE, show_col_types = FALSE)

df <- raw %>%
  transmute(
    Method = .data$Method,
    Mean_us = vapply(.data$Mean, parse_time_to_us, numeric(1)),
    Energy_uJ = vapply(.data$`Package Energy (uJ/op)`, parse_energy_uj, numeric(1))
  ) %>%
  mutate(
    Operation = case_when(
      str_detect(Method, "_Serialize_")   ~ "Serialize",
      str_detect(Method, "_Deserialize_") ~ "Deserialize",
      TRUE ~ NA_character_
    ),
    Size = case_when(
      str_detect(Method, "_Small$")  ~ "Small",
      str_detect(Method, "_Medium$") ~ "Medium",
      str_detect(Method, "_Large$")  ~ "Large",
      TRUE ~ NA_character_
    ),
    Library = Method %>%
      str_replace("_Serialize_.*$", "") %>%
      str_replace("_Deserialize_.*$", "")
  ) %>%
  filter(!is.na(Operation), !is.na(Size)) %>%
  select(Library, Operation, Size, Energy_uJ, Mean_us)

# In case you ever have duplicates (shouldn't), take the mean
df_sum <- df %>%
  group_by(Library, Operation, Size) %>%
  summarise(
    Energy_uJ = mean(Energy_uJ, na.rm = TRUE),
    Mean_us   = mean(Mean_us,   na.rm = TRUE),
    .groups = "drop"
  )

# -------------------------
# LaTeX writer (manual, so we control headers exactly)
# -------------------------
fmt_int <- function(x) ifelse(is.na(x), "--", sprintf("%.0f", x))
fmt_us  <- function(x) ifelse(is.na(x), "--", sprintf("%.1f", x))

write_energy_time_table <- function(operation, out_file, caption, label) {
  t <- df_sum %>%
    filter(Operation == operation) %>%
    mutate(Size = factor(Size, levels = c("Small", "Medium", "Large"))) %>%
    arrange(Library, Size) %>%
    tidyr::pivot_wider(
      names_from = Size,
      values_from = c(Energy_uJ, Mean_us)
    ) %>%
    # Order by Small energy (optional, tends to read nicely)
    arrange(Energy_uJ_Small) %>%
    transmute(
      Library,
      Small_E = fmt_int(Energy_uJ_Small),  Small_T = fmt_us(Mean_us_Small),
      Med_E   = fmt_int(Energy_uJ_Medium), Med_T   = fmt_us(Mean_us_Medium),
      Large_E = fmt_int(Energy_uJ_Large),  Large_T = fmt_us(Mean_us_Large)
    )

  lines <- c(
    "\\begin{table}[H]",
    "\\centering",
    sprintf("\\caption{\\label{%s}%s}", label, caption),
    "\\begin{tabular}{lrrrrrr}",
    "\\toprule",
    " & \\multicolumn{2}{c}{Small} & \\multicolumn{2}{c}{Medium} & \\multicolumn{2}{c}{Large} \\\\",
    "\\cmidrule(lr){2-3}\\cmidrule(lr){4-5}\\cmidrule(lr){6-7}",
    "Library & Energy & Time & Energy & Time & Energy & Time \\\\",
    "\\midrule"
  )

  body <- apply(t, 1, function(r) {
    sprintf("%s & %s & %s & %s & %s & %s & %s \\\\",
            r[["Library"]], r[["Small_E"]], r[["Small_T"]],
            r[["Med_E"]],   r[["Med_T"]],
            r[["Large_E"]], r[["Large_T"]])
  })

  lines <- c(lines, body, "\\bottomrule", "\\end{tabular}", "\\end{table}")

  writeLines(lines, out_file)
  cat("Wrote:", out_file, "\n")
}

# -------------------------
# Export tables
# -------------------------
write_energy_time_table(
  operation = "Serialize",
  out_file  = file.path(OUTPUT_DIR, "table_energy_time_serialize.tex"),
  caption   = "Serialize: package energy (\\(\\mu\\)J/op) and execution time (\\(\\mu\\)s/op) across payload sizes.",
  label     = "tab:energy-time-serialize"
)

write_energy_time_table(
  operation = "Deserialize",
  out_file  = file.path(OUTPUT_DIR, "table_energy_time_deserialize.tex"),
  caption   = "Deserialize: package energy (\\(\\mu\\)J/op) and execution time (\\(\\mu\\)s/op) across payload sizes.",
  label     = "tab:energy-time-deserialize"
)

# =========================
# EnergyOverviewColorTables.R  (FIXED: vectorized cell coloring)
# =========================

suppressPackageStartupMessages({
  library(tidyverse)
})

CSV_FILE   <- "../src/Serialization.Bench/BenchmarkArtifacts/results/Serialization.Bench.JsonSerializationBenchmarks-report.csv"
OUTPUT_DIR <- "graphs/analysis"
dir.create(OUTPUT_DIR, recursive = TRUE, showWarnings = FALSE)

COLOR_MODE <- "energy"   # "energy" | "time" | "both"

# -------------------------
# Parsing helpers
# -------------------------
parse_energy_uJ <- function(x) {
  x <- as.character(x) |> replace_na(NA_character_) |> str_to_lower()
  x <- str_replace_all(x, ",", "")
  x <- str_replace_all(x, "µ", "u")
  x <- str_replace_all(x, "\\s*uj.*$", "")
  x <- str_trim(x)
  suppressWarnings(as.numeric(x))
}

parse_time_to_us <- function(x) {
  s <- trimws(gsub("\"", "", as.character(x)))
  parts <- strsplit(s, "\\s+")[[1]]
  if (length(parts) < 2) return(NA_real_)
  val <- suppressWarnings(as.numeric(gsub(",", "", parts[1])))
  unit <- parts[2]
  if (is.na(val)) return(NA_real_)

  if (unit == "ns") return(val / 1e3)
  if (unit == "us") return(val)
  if (unit == "ms") return(val * 1e3)
  if (unit == "s")  return(val * 1e6)
  NA_real_
}

extract_size <- function(method) {
  case_when(
    str_detect(method, "Small")  ~ "Small",
    str_detect(method, "Medium") ~ "Medium",
    str_detect(method, "Large")  ~ "Large",
    TRUE ~ NA_character_
  )
}

extract_operation <- function(method) {
  case_when(
    str_detect(method, "Serialize")   ~ "Serialize",
    str_detect(method, "Deserialize") ~ "Deserialize",
    TRUE ~ NA_character_
  )
}

extract_tool <- function(method) {
  method %>%
    str_replace_all("_Small|_Medium|_Large", "") %>%
    str_replace_all("_Serialize|_Deserialize", "") %>%
    str_replace_all("__+", "_") %>%
    str_replace_all("^_|_$", "")
}

# -------------------------
# Color helpers (VECTORIZED)
# -------------------------
normalize_block <- function(x) {
  mn <- min(x, na.rm = TRUE)
  mx <- max(x, na.rm = TRUE)
  if (!is.finite(mn) || !is.finite(mx)) return(rep(NA_real_, length(x)))
  if (mx == mn) return(rep(0.5, length(x)))
  (x - mn) / (mx - mn)
}

# green -> red endpoints (Excel-like)
GREEN <- c(198, 239, 206)
RED   <- c(255, 199, 206)

rgb_interp_scalar <- function(s) {
  s <- min(max(s, 0), 1)
  round(GREEN * (1 - s) + RED * s)
}

cell_color_vec <- function(values, s_vec) {
  # values: character vector, s_vec: numeric vector [0..1]
  rgb_mat <- t(vapply(s_vec, rgb_interp_scalar, numeric(3)))
  sprintf("\\cellcolor[RGB]{%d,%d,%d}{%s}",
          rgb_mat[,1], rgb_mat[,2], rgb_mat[,3], values)
}

fmt_energy <- function(x) ifelse(is.na(x), "--", as.character(round(x, 0)))
fmt_time   <- function(x) ifelse(is.na(x), "--", sprintf("%.2f", x))

# -------------------------
# Load + clean
# -------------------------
df_raw <- read_csv(CSV_FILE, show_col_types = FALSE)

energy_col <- "Package Energy (uJ/op)"
stopifnot(energy_col %in% names(df_raw))

df <- df_raw %>%
  transmute(
    Method = .data$Method,
    Tool = extract_tool(.data$Method),
    Operation = extract_operation(.data$Method),
    Size = extract_size(.data$Method),
    Energy_uJ = parse_energy_uJ(.data[[energy_col]]),
    Time_us = vapply(.data$Mean, parse_time_to_us, numeric(1))
  ) %>%
  filter(!is.na(Operation), !is.na(Size)) %>%
  mutate(
    Operation = factor(Operation, levels = c("Serialize", "Deserialize")),
    Size = factor(Size, levels = c("Small", "Medium", "Large"))
  ) %>%
  distinct(Tool, Operation, Size, .keep_all = TRUE)

# Stable ordering (optional, but nice)
tool_order <- df %>%
  group_by(Operation, Tool) %>%
  summarise(avg_energy = mean(Energy_uJ, na.rm = TRUE), .groups = "drop") %>%
  arrange(Operation, avg_energy)

make_table_for_operation <- function(op) {
  d <- df %>% filter(Operation == op) %>%
    mutate(Tool = factor(Tool, levels = tool_order %>% filter(Operation == op) %>% pull(Tool)))

  d_norm <- d %>%
    group_by(Size) %>%  # (Operation fixed here), normalize separately per Size block
    mutate(
      s_energy = normalize_block(Energy_uJ),
      s_time   = normalize_block(Time_us)
    ) %>%
    ungroup()

  # IMPORTANT: use the vectorized cell_color_vec()
  d_cells <- d_norm %>%
    mutate(
      Energy_cell = {
        vals <- fmt_energy(Energy_uJ)
        if (COLOR_MODE %in% c("energy","both")) cell_color_vec(vals, s_energy) else vals
      },
      Time_cell = {
        vals <- fmt_time(Time_us)
        if (COLOR_MODE %in% c("time","both")) cell_color_vec(vals, s_time) else vals
      }
    )

  wide <- d_cells %>%
    select(Tool, Size, Energy_cell, Time_cell) %>%
    pivot_wider(names_from = Size, values_from = c(Energy_cell, Time_cell)) %>%
    arrange(Tool)

  for (sz in c("Small","Medium","Large")) {
    e <- paste0("Energy_cell_", sz); t <- paste0("Time_cell_", sz)
    if (!(e %in% names(wide))) wide[[e]] <- "--"
    if (!(t %in% names(wide))) wide[[t]] <- "--"
  }

  caption <- sprintf(
    "%s: execution time (\\textmu s/op) and package energy (\\textmu J/op) across payload sizes. Cell colors are normalized within each payload size block (green = lowest, red = highest).",
    op
  )
  label <- sprintf("tab:energy-time-%s-colored", tolower(op))

  lines <- c(
    "\\begin{table}[H]",
    "\\centering",
    sprintf("\\caption{\\label{%s}%s}", label, caption),
    "\\begin{tabular}{lrrrrrr}",
    "\\toprule",
    " & \\multicolumn{2}{c}{Small} & \\multicolumn{2}{c}{Medium} & \\multicolumn{2}{c}{Large} \\\\",
    "\\cmidrule(lr){2-3}\\cmidrule(lr){4-5}\\cmidrule(lr){6-7}",
    "Library & Energy & Time & Energy & Time & Energy & Time \\\\",
    "\\midrule"
  )

  for (i in seq_len(nrow(wide))) {
    row <- wide[i, ]
    lines <- c(lines, sprintf(
      "%s & %s & %s & %s & %s & %s & %s \\\\",
      as.character(row$Tool),
      row[["Energy_cell_Small"]],  row[["Time_cell_Small"]],
      row[["Energy_cell_Medium"]], row[["Time_cell_Medium"]],
      row[["Energy_cell_Large"]],  row[["Time_cell_Large"]]
    ))
  }

  lines <- c(lines, "\\bottomrule", "\\end{tabular}", "\\end{table}")
  paste(lines, collapse = "\n")
}

writeLines(make_table_for_operation("Serialize"),
           file.path(OUTPUT_DIR, "table_energy_time_serialize_colored.tex"))
writeLines(make_table_for_operation("Deserialize"),
           file.path(OUTPUT_DIR, "table_energy_time_deserialize_colored.tex"))

cat("Wrote colored tables to:", OUTPUT_DIR, "\n")
cat("COLOR_MODE =", COLOR_MODE, "\n")