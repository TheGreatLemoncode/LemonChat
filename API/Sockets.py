import asyncio
import websockets

async def handler(socket):
    message = await socket.recv()
    print(f"from client: {message}")
    response = "Pathway created"
    await socket.send(response)

async def main():
    async with websockets.serve(handler, "localhost", 8090):
        print("server is running: ...")
        await asyncio.Future()

def run_server():
    asyncio.run(main())
