# Base image with Python 3.12
FROM python:3.12-slim

# Install system dependencies
RUN apt-get update && apt-get install -y --no-install-recommends \
    curl build-essential git \
    && rm -rf /var/lib/apt/lists/*

# Install uv (pip but faster)
RUN curl -LsSf https://astral.sh/uv/install.sh | sh

# Add uv to PATH
ENV PATH="/root/.local/bin:${PATH}"

# Set working directory
WORKDIR /workspaces/app

# Copy dependency files
COPY pyproject.toml ./
COPY uv.lock ./

# Install python dependencies (editable mode for development)
RUN uv pip install --system -r pyproject.toml

# Copy the rest of your project
COPY . .