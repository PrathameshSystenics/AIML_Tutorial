from pydantic import BaseModel, Field
from common import model
from langchain_core.prompts import ChatPromptTemplate


# create the structured output
class QueryOutput(BaseModel):
    query: str = Field(description="Query the user had asked")
    response: str = Field(description="Response returned by the model")


prompt_template = ChatPromptTemplate.from_messages(
    [
        ("system", "you are helpful Assistant"),
        ("human", "What is ai ?")
    ]
)

structured_model = model.with_structured_output(QueryOutput)

model_chain = prompt_template | structured_model

# invoke the chain
response: BaseModel = model_chain.invoke({})

print(response)
