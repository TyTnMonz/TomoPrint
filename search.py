import sys
import fitz  # PyMuPDF

def count_keyword_in_pdf(pdf_path, keyword):
    # Open the PDF file
    doc = fitz.open(pdf_path)
    
    keyword_count = 0
    
    # Loop through each page of the PDF
    for page_num in range(doc.page_count):
        page = doc.load_page(page_num)  # Get each page
        text = page.get_text()  # Extract the text from the page
        
        # Count how many times the keyword appears in the text of the page
        keyword_count += text.lower().count(keyword.lower())
    
    return keyword_count

if __name__ == "__main__":
    # Check if sufficient arguments are provided
    if len(sys.argv) < 3:
        print("Usage: python script.py <pdf_path> <keyword>")
        sys.exit(1)

    # Get the PDF path and keyword from command-line arguments
    pdf_path = sys.argv[1]
    keyword = sys.argv[2]

    # Count the keyword occurrences
    count = count_keyword_in_pdf(pdf_path, keyword)

    # Print the count to standard output (it will be used by the calling program)
    print(count)
