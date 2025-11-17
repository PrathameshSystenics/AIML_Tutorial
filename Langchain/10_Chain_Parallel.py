from dotenv import load_dotenv
import os
from langchain_google_genai import ChatGoogleGenerativeAI
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser
from langchain_core.runnables import RunnableLambda, RunnableParallel,RunnablePassthrough

load_dotenv()

# Load the Model
model = ChatGoogleGenerativeAI(
    model="gemini-2.5-flash",
    google_api_key=os.getenv("GEMINI_API_KEY")
)

# first step for defining the prompt template
prompt_template = ChatPromptTemplate.from_messages([
    ("system", "You are a expert product reviewer. You write detailed and insightful reviews for tech products."),
    ("human", "Write a review about {product}")
])

# first branch for parallel processing
def analyze_pros(review: str):
    pros_template = ChatPromptTemplate.from_messages([
        ("system", "You are an expert product reviewer."),
        ("human", "Analyze the following review and list the pros:\n{review}")
    ])
    return pros_template.format_prompt(review=review)

# second branch for parallel processing
def analyze_cons(review: str):
    cons_template = ChatPromptTemplate.from_messages([
        ("system", "You are an expert product reviewer."),
        ("human", "Analyze the following review and list the cons:\n{review}")
    ])
    return cons_template.format_prompt(review=review)


# simplify branches with LCEL
pros_branch_chain = (RunnableLambda(
    lambda review: analyze_pros(review)) | model | StrOutputParser())

cons_branch_chain = (RunnableLambda(
    lambda review: analyze_cons(review)) | model | StrOutputParser())

# Creating the chain
chain = (
    prompt_template
    | model
    | StrOutputParser()
    | RunnableParallel(pros=pros_branch_chain, cons=cons_branch_chain, review=RunnablePassthrough())
    | RunnableLambda(lambda results: f"Pros:\n{results['pros']}\n\nCons:\n{results['cons']}\nReview: {results['review']}")
)

# Invoke the chain
response = chain.invoke(
    {"product": "The new Micromax Smartphone with advanced AI features"})
print("Final Review Analysis =>", response)
