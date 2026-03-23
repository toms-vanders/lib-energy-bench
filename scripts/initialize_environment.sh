#!/bin/bash

set -e  # Exit immediately if a command fails

echo "Updating package lists..."
sudo apt-get update

echo "Installing required packages..."
sudo apt-get install -y \
    git \
    dotnet-sdk-8.0 \
    r-base \
    r-cran-ggplot2 \
    r-cran-dplyr \
    r-cran-tidyr \
    r-cran-scales

echo "Setting permissions for intel-rapl..."
sudo chmod -R a+r /sys/class/powercap/intel-rapl || true

echo "Cloning BenchmarkDotNet.Energy repository into parent directory..."
cd ../..

if [ ! -d "BenchmarkDotNet.Energy" ]; then
    git clone https://github.com/geovoda/BenchmarkDotNet.Energy.git
else
    echo "Repository already exists, skipping clone."
fi

cd BenchmarkDotNet.Energy

echo "Checking out feat/rapl-diagnoser branch..."
git fetch
git checkout feat/rapl-diagnoser

echo "Setup completed successfully."
