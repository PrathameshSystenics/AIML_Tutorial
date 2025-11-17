from langchain_community.document_loaders import TextLoader
import os
from dotenv import load_dotenv
from langchain_core.documents.base import Document
from langchain_text_splitters import CharacterTextSplitter
from langchain_google_genai.embeddings import GoogleGenerativeAIEmbeddings
from langchain_chroma import Chroma

load_dotenv()

current_dir = os.path.dirname(os.path.abspath(__file__))
file_path = os.path.join(current_dir, "documents", "odessy.txt")
persistent_directory = os.path.join(current_dir, "vectorstore_db")

# create the embeddings
embeddings = GoogleGenerativeAIEmbeddings(
    google_api_key=os.getenv("GEMINI_API_KEY"),
    model="gemini-embedding-001"
)
# generate embeddings for the chunks and store in vectorstore in the chrome
vectorstore = Chroma(
    embedding_function=embeddings,
    persist_directory=persistent_directory,
    collection_name="odessy_collection",
)

# search for similar documents in the vectorstore
query = "What is the story about?"

# create the retriever from the vectorstore
retriever = vectorstore.as_retriever(
    search_type="similarity",
    search_kwargs={"k": 2}  # number of similar documents to retrieve
)

relevant_docs: list[Document]= retriever.invoke(query)

print("\n--- Retrieved Documents ---")
for i, doc in enumerate(relevant_docs):
    print(f"Document {i+1}:\n{doc.page_content}\n")
    # You can even get the metadata if needed.
    print(f"Metadata: {doc.metadata}\n")
