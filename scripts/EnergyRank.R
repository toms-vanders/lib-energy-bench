# =========================
# EnergyRank
# Plots BenchmarkDotNet benckmarks as a lollipop chart to show the energy consumption of serialization and deserealization, grouped by payload size.
# =========================

library(tidyverse)

# -------------------------
# Config
# -------------------------
CSV_FILE <- "../src/Serialization.Bench/BenchmarkArtifacts/results/Serialization.Bench.JsonSerializationBenchmarks-report.csv"

# -------------------------
# The plotting function
# -------------------------
plot_energy_lollipop <- function(csv_file, filter_string) {
  # Read and clean
  df <- read_csv(csv_file, show_col_types = FALSE) %>%
    # Keep only rows with "Small" in Method
    filter(str_detect(Method, filter_string)) %>%
    mutate(
      Energy = `Package Energy (uJ/op)` %>%
        as.character() %>%
        replace_na("0") %>%
        str_remove_all(",") %>%
        str_remove_all(" uj") %>%
        as.numeric(),
      # Extract operation: "Serialize" or "Deserialize"
      Operation = if_else(str_detect(Method, "Serialize"), "Serialize", "Deserialize"),
      MethodLabel = str_replace_all(Method, "_", " ") %>%
        str_replace_all(filter_string, "")
    ) %>%
    # Sort factor by Operation and Energy
    arrange(Operation, desc(Energy)) %>%
    mutate(MethodLabel = factor(MethodLabel, levels = MethodLabel))  # preserve sorted order

  shape_map <- c("Deserialize" = 16, "Serialize" = 15) # Deserialize head = Circle; Serialize head = Square

  # Plot
  p <- ggplot(df, aes(x = MethodLabel, y = Energy, color = Operation, shape = Operation)) +
    geom_segment(aes(x = MethodLabel, xend = MethodLabel, y = 0, yend = Energy), size = 1, alpha = 0.5, linewidth = 2, show.legend = FALSE) +
    geom_point(size = 4, alpha = 0.8) +
    geom_text(aes(x = MethodLabel, y = Energy, label = round(Energy, 0)), hjust = 0.5, vjust =-0.8, size = 4.5, inherit.aes = FALSE) +
    scale_shape_manual(values = shape_map) +
    coord_flip() +
    labs(
      title = paste("Package Energy per Operation (", filter_string, " JSON)"),
      x = "Method",
      y = "Energy (µJ / operation)",
      color = "Operation"
    ) +
    theme_light() +
    theme(
      panel.grid.major.y = element_blank(),
      panel.border = element_blank(),
      axis.ticks.y = element_blank(),
      axis.title = element_text(size=14), 
      axis.text.x = element_text(size=12), 
      axis.text.y = element_text(size=12), 
      axis.title.x = element_text(margin = margin(t = 15, b = 10)),
      axis.title.y = element_text(margin = margin(l = 10, r = 15)),
      plot.title = element_text(size=17, face="bold", hjust=0.5, margin = margin(t = 10, b = 10)),  # centered title
      legend.text = element_text(size = 11), 
      legend.title = element_text(size = 14), 
    )

  return(p)
}

# -------------------------
# Plots export
# -------------------------
p <- plot_energy_lollipop(CSV_FILE, "Small")
ggsave("graphs/JsonToolsRankSmall.png", p, width = 11, height = 6, dpi = 150, create.dir = TRUE)

p <- plot_energy_lollipop(CSV_FILE, "Medium")
ggsave("graphs/JsonToolsRankMedium.png", p, width = 11, height = 6, dpi = 150, create.dir = TRUE)

p <- plot_energy_lollipop(CSV_FILE, "Large")
ggsave("graphs/JsonToolsRankLarge.png", p, width = 11, height = 6, dpi = 150, create.dir = TRUE)
