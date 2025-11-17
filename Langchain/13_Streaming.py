from common import model
from langchain_core.prompts import ChatPromptTemplate
from langchain_core.output_parsers import StrOutputParser

# creating a prompt template
prompt_template = ChatPromptTemplate.from_messages([
    ("system", "You are a expert product reviewer. You write detailed and insightful reviews for tech products. Review the product in one paragraph only"),
    ("human", "Write a review about {product}")
])

# Invoke the prompt
chain = prompt_template | model | StrOutputParser()

# streaming the response
async def stream_response():
    response = chain.astream({"product": "Oneplus 15 phone"})
    async for chunk in response:
        print(chunk, end="", flush=True)
        
import asyncio
asyncio.run(stream_response())