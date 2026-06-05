import jwt
from jwt.exceptions import *

SECRET_KEY = "d48dcddea6a3695bf2dae1bdf317048044ceb96a491c232956715c3e177f1666"

__ALL__ = [
    "create_jwt_token",
    "decrypt_token"
]

def create_jwt_token(payload) -> str:
    """Create a jwt token with the payload given as argument"""
    token = jwt.encode(payload, SECRET_KEY, algorithm="HS256")
    return token

def decrypt_token(user_token: str, service : str):
    """
    Verify the given jwt token as string and check if it's for the given 
    audience. return the decoded payload or raise an execption
    """
    decode = jwt.decode(user_token, SECRET_KEY, algorithms=["HS256"]
                            , audience= service, issuer="API.me")   
    print(f"the payload incripted was : \n{decode}")
    return decode
