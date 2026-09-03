# MLClassProject
Class Project for CS Machine Learning A Term 2026 

## Python setup (ML-Agents training)

Everyone needs the **same** Python and package versions, because the Python trainer has to match the Unity package (`com.unity.ml-agents` 4.1.0). This repo pins both, so nobody installs Python by hand:

- `.python-version` pins **Python 3.10.12** (the only range ML-Agents supports is 3.10.1 - 3.10.12).
- `pyproject.toml` + `uv.lock` pin the ML-Agents Python packages and every transitive dependency.

We use [uv](https://docs.astral.sh/uv/) to manage this. It downloads the pinned Python for you.

### One-time setup per machine

1. Install uv:
   - macOS: `brew install uv` (or `curl -LsSf https://astral.sh/uv/install.sh | sh`)
   - Windows (PowerShell): `powershell -ExecutionPolicy ByPass -c "irm https://astral.sh/uv/install.ps1 | iex"`
2. From the **repo root** (the folder with this README), run:
   ```
   uv sync
   ```
   This downloads Python 3.10.12, creates `.venv/` (git-ignored), and installs exactly what is in `uv.lock`.
3. Check it worked:
   ```
   uv run mlagents-learn --help
   ```

### Training

Run commands through `uv run` so they always use the project's Python. From the repo root:

```
uv run mlagents-learn <path/to/config.yaml> --run-id=<run_name>
```

Then press Play in the Unity Editor. Training output lands in `results/` (git-ignored).

If you prefer an activated shell instead of `uv run`: `source .venv/bin/activate` (macOS/Linux) or `.venv\Scripts\activate` (Windows).

### Rules

- Do not `pip install` into the venv by hand. To add or change a dependency, edit `pyproject.toml`, run `uv lock`, and commit the updated `uv.lock`.
- After pulling changes that touch `pyproject.toml` or `uv.lock`, run `uv sync` again.
- Apple Silicon Macs: every `uv sync` / `uv run` prints `~ grpcio==1.53.2` (a reinstall). This is a harmless quirk of grpcio's macOS wheel being mislabeled as x86_64 internally; ignore it.
- Optional, Windows + NVIDIA GPU: the default install is CPU-only PyTorch, which is fine for this project. See the PyTorch install page if you want CUDA.
