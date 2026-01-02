# =========================
# EnergyRankEvolution.R
# Plots BenchmarkDotNet benckmarks as a bump chart to show how energy consumption evolves when increasing the payload size.
# =========================

library(tidyverse)
library(ggbump)

# -------------------------
# Config
# -------------------------
CSV_FILE <- "../src/Serialization.Bench/BenchmarkArtifacts/results/Serialization.Bench.JsonSerializationBenchmarks-report.csv"

# -------------------------
# The plotting function
# -------------------------
plot_energy_bump <- function(csv_file, operation_filter) {

  # Read and clean data
  df <- read_csv(csv_file, show_col_types = FALSE) %>%
    filter(str_detect(Method, operation_filter)) %>%
    mutate(
      # Convert Energy to numeric
      Energy = `Package Energy (uJ/op)` %>%
        as.character() %>%
        replace_na("0") %>%
        str_remove_all(",") %>%
        str_remove_all(" uj") %>%
        as.numeric(),
      # Extract Size
      Size = case_when(
        str_detect(Method, "Small") ~ "Small",
        str_detect(Method, "Medium") ~ "Medium",
        str_detect(Method, "Large") ~ "Large"
      ),
      # Extract Tool name without size or operation
      Tool = Method %>%
        str_replace_all("_Small|_Medium|_Large", "") %>%
        str_replace_all("_Deserialize|_Serialize", "")
    ) %>%
    select(Tool, Size, Energy)

  # Make sure every Tool has all sizes
  df <- df %>%
    complete(Tool, Size = c("Small", "Medium", "Large")) %>%
    arrange(Tool, Size) %>%
    # Replace missing energy with Inf so rank is at bottom
    mutate(Energy = ifelse(is.na(Energy), Inf, Energy)) %>%
    group_by(Size) %>%
    mutate(Rank = rank(Energy, ties.method = "first")) %>%
    ungroup() %>%
    mutate(Size = factor(Size, levels = c("Small", "Medium", "Large")))

  # Labels for all points
  df_labels <- df %>%
    filter(Size == "Small" & !is.infinite(Energy)) %>%
    mutate(hjust_label = 1.3)

  # Bump plot
  p <- ggplot(df, aes(x = Size, y = Rank, color = Tool, group = Tool, shape=Tool)) +
    geom_bump(size = 2, smooth = 8, show.legend = FALSE) +
    geom_point(size = 4) +
    scale_shape_manual(values = c(16, 17, 15, 18, 21)) +
    geom_text(data = df_labels, aes(label = Tool), nudge_x = -0.03, hjust = 1, vjust = 0.5, size = 4, show.legend = FALSE) +
    scale_x_discrete(expand = expansion(mult = c(0.15, 0.05))) +
    scale_y_reverse() +
    labs(
      title = paste("Evolution of Energy Ranking with Increasing JSON Size", "(", operation_filter, ")"),
      x = "Input Size",
      y = "Energy rank (1 = lowest energy)",
      color = "Tool"
    ) +
    theme_light() +
    theme(
      panel.grid.major.y = element_blank(),
      panel.grid.minor.y = element_blank(),
      panel.border = element_blank(),
      plot.title = element_text(size=17, face="bold", hjust=0.5, margin = margin(t = 10, b = 10)),  # centered title
      axis.title = element_text(size=14),  # axis labels
      axis.text.x = element_text(size=12), 
      axis.text.y = element_text(size=12), 
      axis.title.x = element_text(margin = margin(t = 15, b = 10)),
      axis.title.y = element_text(margin = margin(l = 10, r = 15))
    )

  return(p)
}

# -------------------------
# Plots export
# -------------------------
bump_serialize <- plot_energy_bump(CSV_FILE, "Serialize")
ggsave("graphs/JsonToolsRankEvolutionSerialize.png", bump_serialize, width = 11, height = 4, dpi = 150, create.dir = TRUE)

bump_deserialize <- plot_energy_bump(CSV_FILE, "Deserialize")
ggsave("graphs/JsonToolsRankEvolutionDeserialize.png", bump_deserialize, width = 11, height = 4, dpi = 150, create.dir = TRUE)
