import asyncio
import websockets
from Security import token
from jwt.exceptions import *

Users = dict()

async def on_connect(socket):
    x_token = socket.request.headers.get("X-token-socket")
    try:
        decode = token.decrypt_token(x_token, "chat-service")
        Users[decode["sub"]] = socket
    except InvalidAudienceError:
        print("invalide audience")
    except InvalidIssuerError:
        print("invalide issuer")
    await socket.send("welcome")

async def on_message(socket, message):
    print(f"Message from client: {message}")

async def on_disconnect(socket):
    print("Connection closed")

async def handler(socket):
    await on_connect(socket)
    try:
        async for messages in socket:
            await on_message(socket, messages)
    except websockets.exceptions.ConnectionClosed:
        await on_disconnect(socket)

async def main():
    async with websockets.serve(handler, "localhost", 8090):
        print("server is running: ...")
        await asyncio.Future()

def run_server():
    asyncio.run(main())
