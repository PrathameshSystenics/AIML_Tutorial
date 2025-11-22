import os
from dotenv import load_dotenv
load_dotenv()

from src.agent import react_graph
from langchain.messages import HumanMessage


print(os.getenv("GOOGLE_API_KEY"))

messages = [HumanMessage(content="Add 3 and 4. Multiply the output by 2. subtract the output by 5")]
messages = react_graph.invoke({"messages": messages})
print(messages)