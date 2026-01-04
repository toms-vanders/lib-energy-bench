# =========================
# EnergyCompareUnified
# Single plot showing energy per operation
# Serialize vs Deserialize
# For Small / Medium / Large payloads
# =========================

library(tidyverse)

# -------------------------
# Config
# -------------------------
CSV_FILE <- "../src/Serialization.Bench/BenchmarkArtifacts/results/Serialization.Bench.JsonSerializationBenchmarks-report.csv"

# -------------------------
# Load and clean data
# -------------------------
df <- read_csv(CSV_FILE, show_col_types = FALSE) %>%
  filter(str_detect(Method, "Small|Medium|Large")) %>%
  mutate(
    Energy = `Package Energy (uJ/op)` %>%
      as.character() %>%
      replace_na("0") %>%
      str_remove_all(",") %>%
      str_remove_all(" uj") %>%
      as.numeric(),

    Operation = if_else(
      str_detect(Method, "Serialize"),
      "Serialize",
      "Deserialize"
    ),

    Size = case_when(
      str_detect(Method, "Small") ~ "Small",
      str_detect(Method, "Medium") ~ "Medium",
      str_detect(Method, "Large") ~ "Large"
    ),

    Library = str_extract(Method, "^[^_]+")
  ) %>%
  filter(
    !is.na(Energy),
    Energy > 0,
    !is.na(Size),
    !is.na(Library)
  )

# -------------------------
# Prepare factors
# -------------------------
df <- df %>%
  mutate(
    Operation = factor(Operation, levels = c("Deserialize", "Serialize")),
    Size = factor(Size, levels = c("Small", "Medium", "Large"))
  )

# -------------------------
# Plot
# -------------------------
p <- ggplot(
  df,
  aes(
    x = Operation,
    y = Energy,
    group = interaction(Library, Size),
    color = Operation
  )
) +
  geom_line(linewidth = 1.1, alpha = 0.8) +
  geom_point(size = 3, alpha = 0.9) +
  facet_wrap(~ Size, scales = "free_y") +
  scale_color_manual(
    values = c(
      "Deserialize" = "#1f77b4",
      "Serialize"   = "#d62728"
    )
  ) +
  labs(
    title = "Energy per Operation — Serialization vs Deserialization",
    subtitle = "All JSON libraries, grouped by payload size",
    x = "Operation",
    y = "Energy (µJ / operation)",
    color = "Operation"
  ) +
  theme_light() +
  theme(
    panel.grid.minor = element_blank(),
    strip.text = element_text(size = 14, face = "bold"),
    plot.title = element_text(
      size = 18,
      face = "bold",
      hjust = 0.5,
      margin = margin(t = 10, b = 5)
    ),
    plot.subtitle = element_text(
      size = 13,
      hjust = 0.5,
      margin = margin(b = 10)
    ),
    axis.title = element_text(size = 14),
    axis.text = element_text(size = 12),
    legend.title = element_text(size = 14),
    legend.text = element_text(size = 12),
    legend.position = "bottom"
  )

# -------------------------
# Export
# -------------------------
ggsave(
  "graphs/JsonEnergySerializeVsDeserialize_AllSizes.png",
  p,
  width = 11,
  height = 4,
  dpi = 150,
  create.dir = TRUE
)
