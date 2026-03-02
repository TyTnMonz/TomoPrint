
# Document Print Utility with Command Line Args and Silent Print

## Overview
This tool automates the printing of PDF documents using **[SumatraPDF](https://www.sumatrapdfreader.org)**.

It supports various customization options, including specific printer selection, copy count, page size, and color. 
Additionally, it allows for keyword-based actions by integrating with a Python script to search PDFs for specific keywords and adjust print settings accordingly.

The printing process can be **silent** (without showing any UI).

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
| `--document`        | `-d`  | Path to the PDF document to print.                                                                       | -                |
| `--folder`          | `-f`  | Path to a folder containing documents to print.                                                          | -                |
| `--extension`       | `-e`  | File extension to filter (only used with `--folder`).                                                    | `pdf`            |
| `--printer`         | `-p`  | Name of the printer to use.                                                                              | `Default printer`|
| `--copies`          | `-c`  | Number of copies to print.                                                                               | `1`              |
| `--size`            | `-a`  | Paper size (e.g., A4, Letter).                                                                           | `A4`             |
| `--color`           | `-l`  | Print in color (`true`) or black and white (`false`).                                                    | `false`          |
| `--timer`           | `-t`  | Interval (in seconds) for repeated printing.                                                             | `0 (disabled)`   |
| `--watcher`         | `-w`  | Enable folder monitoring to print new files.                                                             | `false`          |
| `--silent`          | `-s`  | Enable silent printing (no dialogs).                                                                     | `true`           |
| `--output`          | `-o`  | Folder to move printed files after printing.                                                             | `printed`        |
| `--keywordSearch`   | `-k`  | Keyword-based rule (`<keyword,trigger,copies>`).                                                         | -                |
| `--filePerSession`  | `-x`  | Maximum number of files printed per session. Use this to limit how many documents are printed at once when many files are loaded in the folder. Must be greater than 0. | `-1 (print all)`      |

---

## Requirements
- **.NET Core SDK** (latest version recommended).
- **SumatraPDF** executable in the program’s directory. You can download the Portable Version from here [Download SumatraPDF](https://www.sumatrapdfreader.org/download-free-pdf-viewer).
- **Python 3.x** installed (path defined in `config.cfg`) and **[PyMuPDF](https://github.com/pymupdf/PyMuPDF)** library.
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

## Usage Examples, from Bash

1. **Print a single document using the default printer:**
   ```bash
   TomoPrint.exe -d "C:\Documents\file.pdf"
   ```

2. **Print all PDF documents in a folder with 2 copies each:**
   ```bash
   TomoPrint.exe -f "C:\Documents\PDFs" -c 2
   ```

3. **Print the first 20 PDF documents in a folder with 2 copies each, every 5 minutes.**
   
   When set, the 'Files Per Session' option allows you to print a subset of the documents present inside the folder.
   This is useful to prevent starting an indefinitely long print job when a maximum number of documents are uploaded into the folder within a short time frame.
   By setting a limit—possibly in combination with the execution timeout—it allows for better control of the print queue:
   ```bash
   TomoPrint.exe -f "C:\Documents\PDFs" -c 2 -x 20 -t 300
   ```

5. **Silently print all PDF documents in a folder with 2 copies each and activate the Watchdog on that folder (every time a new PDF is added, it will be immediately printed):**
   ```bash
   TomoPrint.exe -f "C:\Documents\PDFs" -c 2 -w -s
   ```

4. **Print a document and trigger a keyword-based rule (keyword: 'Transport', trigger: 3 occurrences, 5 copies). This means that 5 copies of the PDF will be printed if the keyword 'Transport' is found 3 times (the trigger) inside the document.**
   ```bash
   TomoPrint.exe -d "C:\Documents\invoice.pdf" -k "Transport,3,5"
   ```
