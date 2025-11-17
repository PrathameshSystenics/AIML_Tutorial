# creating the documents
import os
from dotenv import load_dotenv
from langchain_chroma import Chroma
from langchain_core.documents import Document
from langchain_google_genai import GoogleGenerativeAIEmbeddings

list_of_docs = [
    Document(
        page_content="The UltraTech X5 Gaming Laptop features an Intel i7 processor, 16GB RAM, and an NVIDIA RTX 4060 GPU.",
        metadata={"product_id": "laptop_101"}
    ),
    Document(
        page_content="The HyperDrive 1TB NVMe SSD offers blazing-fast read speeds up to 7000MB/s, ideal for gaming and productivity.",
        metadata={"product_id": "ssd_203"}
    ),
    Document(
        page_content="The Quantum Mechanical RGB Keyboard includes customizable lighting, mechanical switches, and USB-C connectivity.",
        metadata={"product_id": "keyboard_330"}
    ),
    Document(
        page_content="The AeroCool 750W Gold Power Supply provides stable power delivery and high efficiency for modern gaming PCs.",
        metadata={"product_id": "psu_750"}
    ),
    Document(
        page_content="Our 27-inch 144Hz IPS Gaming Monitor offers vibrant color accuracy and smooth motion for competitive gaming.",
        metadata={"product_id": "monitor_144"}
    ),
    Document(
        page_content="The TitanX Pro Desktop PC includes a Ryzen 7 CPU, 32GB DDR5 RAM, and dual-fan liquid cooling.",
        metadata={"product_id": "desktop_900"}
    ),
    Document(
        page_content="The SteelCore Mid-Tower PC Case features tempered glass panels, RGB fans, and extensive cable management.",
        metadata={"product_id": "case_502"}
    ),
    Document(
        page_content="The PowerBoost 32GB DDR5 memory kit operates at 6000MHz, making it perfect for high-performance builds.",
        metadata={"product_id": "ram_32gb"}
    ),
    Document(
        page_content="The StormMax Wireless Mouse provides adjustable DPI settings and ergonomic design for long gaming sessions.",
        metadata={"product_id": "mouse_440"}
    ),
    Document(
        page_content="The CoolFlow Liquid AIO Cooler includes a 240mm radiator, dual ARGB fans, and silent pump technology.",
        metadata={"product_id": "cooler_240"}
    ),
    Document(
        page_content="The ThunderBolt USB-C Hub expands your laptop with HDMI, USB 3.0, SD card reader, and charging port.",
        metadata={"product_id": "hub_220"}
    ),
    Document(
        page_content="Our EcoPack shipping ensures secure packaging for computer components with shock-resistant foam.",
        metadata={"category": "shipping_info"}
    ),
    Document(
        page_content="Customers receive a 2-year warranty on all computer parts, including CPUs, motherboards, and GPUs.",
        metadata={"category": "warranty_info"}
    ),
    Document(
        page_content="The FlexiMount Monitor Arm supports dual displays and offers tilt, swivel, and height adjustments.",
        metadata={"product_id": "mount_120"}
    ),
    Document(
        page_content="The OmegaSound Gaming Headset delivers immersive 7.1 surround sound and noise-cancelling microphone.",
        metadata={"product_id": "headset_710"}
    ),
    Document(
        page_content="Our Express Delivery guarantees 2-day shipping on select laptops and pre-built desktop systems.",
        metadata={"category": "delivery_policy"}
    ),
    Document(
        page_content="The FusionPro Motherboard supports AM5 CPUs, PCIe 5.0 graphics cards, and DDR5 memory modules.",
        metadata={"product_id": "motherboard_550"}
    ),
    Document(
        page_content="The RapidCharge 65W USB-C Adapter supports fast charging for laptops, tablets, and smartphones.",
        metadata={"product_id": "charger_65w"}
    ),
    Document(
        page_content="The GraphiMax RTX 4070 GPU includes 12GB GDDR6 memory and supports ray tracing and DLSS 3.0.",
        metadata={"product_id": "gpu_4070"}
    ),
    Document(
        page_content="The UltraView Webcam delivers 1080p HD video and automatic low-light correction for streaming and meetings.",
        metadata={"product_id": "webcam_1080"}
    )
]

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

vectorstore.add_documents(list_of_docs)