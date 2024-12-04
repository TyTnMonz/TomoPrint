
# Document Print Utility with Keyword-Based Rules

## Overview
This program is a C# application designed to automate the printing of PDF documents using **SumatraPDF**. It supports various customization options, including specific printer selection, copy count, page size, and color. Additionally, it allows for keyword-based actions by integrating with a Python script to search PDFs for specific keywords and adjust print settings accordingly.

The program can also monitor a folder and automatically print new documents or periodically reprint files at specified intervals.

---

## Features
- Print a single document or all documents in a folder.
- Set a specific printer or use the system default.
- Specify the number of copies, paper size, and color mode.
- Monitor a folder for new files and print them automatically.
- Perform keyword searches in documents to trigger custom print settings.
- Move printed files to a designated output folder.

---

## Command-Line Arguments

| Option              | Alias | Description                                                                                              | Default          |
|---------------------|-------|----------------------------------------------------------------------------------------------------------|------------------|
| `--document`        | `-d`  | Path to the PDF document to print.                                                                        | -                |
| `--folder`          | `-f`  | Path to a folder containing documents to print.                                                           | -                |
| `--extension`       | `-e`  | File extension to filter (only used with `--folder`).                                                     | `pdf`            |
| `--printer`         | `-p`  | Name of the printer to use.                                                                               | Default printer  |
| `--copies`          | `-c`  | Number of copies to print.                                                                                | 1                |
| `--size`            | `-a`  | Paper size (e.g., A4, Letter).                                                                            | `A4`             |
| `--color`           | `-l`  | Print in color (`true`) or black and white (`false`).                                                     | `false`          |
| `--timer`           | `-t`  | Interval (in seconds) for repeated printing.                                                              | 0 (disabled)     |
| `--watcher`         | `-w`  | Enable folder monitoring to print new files.                                                              | `false`          |
| `--silent`          | `-s`  | Enable silent printing (no dialogs).                                                                      | `true`           |
| `--output`          | `-o`  | Folder to move printed files after printing.                                                              | `printed`        |
| `--keywordSearch`   | `-k`  | Keyword-based rule (`<keyword,trigger,copies>`).                                                          | -                |

---

## Requirements
- **.NET Core SDK** (latest version recommended).
- **SumatraPDF** executable in the program’s directory.
- **Python 3.x** installed (path defined in `config.cfg`).
- Python script (`search.py`) in the program’s directory.

---

## Installation
1. Clone the repository:
   ```bash
   git clone <repository-url>
   cd <repository-folder>
   ```

2. Create a `config.cfg` file in the program’s directory with the following content:
   ```cfg
   python_path=<path-to-python-executable>
   ```

3. Build and run the program:
   ```bash
   dotnet build
   dotnet run -- <arguments>
   ```

---

## Usage Examples

1. **Print a single document using the default printer:**
   ```bash
   dotnet run -- -d "C:\Documents\file.pdf"
   ```

2. **Print all PDF documents in a folder with 2 copies each:**
   ```bash
   dotnet run -- -f "C:\Documents\PDFs" -c 2
   ```

3. **Print a document and trigger a keyword-based rule (keyword: "Transport", trigger: 3 occurrences, 5 copies):**
   ```bash
   dotnet run -- -d "C:\Documents\invoice.pdf" -k "Transport,3,5"
   ```
