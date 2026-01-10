import hashlib, os
import jwt

__ALL__ = [
    "create_jwt_token",
    "hash_password"
]

SECRET_KEY = "theflowerswesawthatday"

def create_jwt_token(payload) -> str:
    """Create a jwt token with the payload given as argument"""
    token = jwt.encode(payload, SECRET_KEY, algorithm="HS256")
    return token

def hash_password(Password: bytes, salt : bytes) -> str:
    """hash a clear password a return it plus the salt used"""
    hashed = hashlib.scrypt(password=Password, salt=salt, p=8, dklen= 64, n=16384, r=8)
    return hashed.hex()

def compare_hash_clear(clear: str, salt: str, hash : str) -> bool:
    """Compare the clear password with the hash using a salt"""
    new_hash = hash_password(clear.encode("utf-8"), salt.encode("utf-8"))
    return bool(new_hash == hash)


def create_salt():
    """create a 16 bytes salt to hash password"""
    buffer = os.urandom(16)
    return buffer
