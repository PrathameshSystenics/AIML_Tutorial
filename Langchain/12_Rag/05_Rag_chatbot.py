
from urllib import response
from dotenv import load_dotenv
import os
from langchain_chroma import Chroma
from langchain_google_genai import ChatGoogleGenerativeAI, GoogleGenerativeAIEmbeddings
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_core.runnables import RunnablePassthrough

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

model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

query = "do we have gaming laptop"

retriever = vectorstore.as_retriever(
    search_type="similarity", search_kwargs={"k": 3})

prompt = """You are a helpful assistant for a computer parts store. Use the following context to answer the question. If you do not have information then respond as "dont'know.
Context:
{context}
"""

prompt_template = ChatPromptTemplate.from_messages([
    ("system", prompt),
    ("user", "{question}")
])

response = ({
    "context": retriever,
    "question": RunnablePassthrough()
}
    | prompt_template
    | model
    | StrOutputParser())

final_response = response.invoke(query)
print("Final Response:" + final_response)
