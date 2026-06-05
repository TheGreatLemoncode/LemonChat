import asyncio
import websockets

async def Connect():
    async with websockets.connect("ws://localhost:8090") as ws:
        message = "hello from client"
        await ws.send(message)
        response = await ws.recv()
        print(f"message: {response}")


if __name__ == "__main__":
    asyncio.run(Connect())
