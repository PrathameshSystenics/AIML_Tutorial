
from dotenv import load_dotenv
import os
from langchain_chroma import Chroma
from langchain_google_genai import GoogleGenerativeAIEmbeddings

load_dotenv()
current_dir = os.path.dirname(os.path.abspath(__file__))
persistent_directory = os.path.join(current_dir, "vectorstore_db_metadata")

# create the embeddings
embeddings = GoogleGenerativeAIEmbeddings(
    google_api_key=os.getenv("GEMINI_API_KEY"),
    model="gemini-embedding-001"
)
# generate embeddings for the chunks and store in vectorstore in the chrome
vectorstore = Chroma(
    embedding_function=embeddings,
    persist_directory=persistent_directory,
    collection_name="computers_metadata_collection",
)

query = "Gaming Computer with best performance"

retriever = vectorstore.as_retriever(search_type="similarity", search_kwargs={"k": 3})

retrieved_docs = retriever.invoke(query)

for i, doc in enumerate(retrieved_docs):
    print(f"Document {i+1}:")
    print(f"Content: {doc.page_content}")
    print(f"Metadata: {doc.metadata}")
    print("-" * 50)
