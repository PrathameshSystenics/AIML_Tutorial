from langchain.tools import tool

@tool
def add(a: int, b: int) -> int:
    """
    Add two integers.

    Parameters
    ----------
    a : int
        First number.
    b : int
        Second number.

    Returns
    -------
    int
        The sum of a and b.
    """
    return a + b


@tool
def subtract(a: int, b: int) -> int:
    """
    Subtract two integers.

    Parameters
    ----------
    a : int
        First number.
    b : int
        Second number.

    Returns
    -------
    int
        The result of a - b.
    """
    return a - b


@tool
def multiply(a: int, b: int) -> int:
    """
    Multiply two integers.

    Parameters
    ----------
    a : int
        First number.
    b : int
        Second number.

    Returns
    -------
    int
        The product of a and b.
    """
    return a * b


tools=[add,multiply,subtract]