from langchain_community.document_loaders import TextLoader
import os
from dotenv import load_dotenv
from langchain_text_splitters import CharacterTextSplitter
from langchain_google_genai.embeddings import GoogleGenerativeAIEmbeddings
from langchain_chroma import Chroma

load_dotenv()

current_dir = os.path.dirname(os.path.abspath(__file__))
file_path = os.path.join(current_dir, "documents", "odessy.txt")
persistent_directory = os.path.join(current_dir, "vectorstore_db")

# Read the Text Content from the file
loader = TextLoader(file_path=file_path,encoding="utf-8")
documents = loader.load()

# split the documents into chunks
text_splitter = CharacterTextSplitter(chunk_size=1000, chunk_overlap=0)
chunks = text_splitter.split_documents(documents)

print("\n--- Document Chunks Information ---")
print(f"Number of document chunks: {len(chunks)}")
print(f"Sample chunk:\n{chunks[0].page_content}\n")

# create the embeddings
embeddings = GoogleGenerativeAIEmbeddings(
    google_api_key=os.getenv("GEMINI_API_KEY"),
    model="gemini-embedding-001"
)
# generate embeddings for the chunks and store in vectorstore in the chrome
vectorstore = Chroma(
    embedding_function=embeddings,
    persist_directory=persistent_directory,
    collection_name="odessy_collection"
)
vectorstore.add_documents(chunks)

print("\n--- Vector Store Information ---")
print(f"Persist directory: {persistent_directory}")
print(f"Collection name: odessy_collection")