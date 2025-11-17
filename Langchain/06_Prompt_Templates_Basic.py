from dotenv import load_dotenv
from langchain_core.prompts import PromptTemplate, ChatPromptTemplate
from langchain.messages import HumanMessage

# Create the Template with Variables
template = "Tell me a joke about {topic}"
prompt: ChatPromptTemplate = ChatPromptTemplate.from_template(
    template=template)

print("----------- Prompt From Template --------")
prompt = prompt.invoke({"topic": "computers"})
# Returns a HumanMessage
print(prompt)


# Prompt with system and Human Message
chat_prompt = ChatPromptTemplate.from_messages([
    ("system", "You are a funny assistant that tells jokes."),
    ("human", "Tell me a joke about {topic}")
])

print("----------- Prompt From Template --------")
prompt = chat_prompt.invoke({"topic": "computers"})
# Returns a HumanMessage
print(prompt)