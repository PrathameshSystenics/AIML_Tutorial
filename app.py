import tkinter as tk
from tkinter import Button
from tkinter import messagebox


def buttonClick():
    messagebox.showinfo("You Clicked the Button", "You Clicked the button")


tk = tk.Tk("Hello World")
tk.title("hello world")
tk_button = Button(text="Click Me", command=buttonClick)
tk_button.place(x=10, y=10)
tk.mainloop()
