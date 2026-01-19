import hashlib, os
from hmac import compare_digest
import Bd

__ALL__ = [
    "hash_password",
    "user_connexion",
    "create_salt"
]


def hash_password(Password: bytes, Salt : bytes) -> str:
    """hash a clear password a return it plus the salt used"""
    hashed = hashlib.scrypt(password=Password, salt=Salt, p=8, dklen= 64, n=16384, r=8)
    return hashed.hex()

def __compare_hash_clear(clear: str, salt: str, hashed : str) -> bool:
    """Compare the clear password with the hash using a salt"""
    b_hashed = bytes.fromhex(hashed)
    b_salts = bytes.fromhex(salt)
    new_hash = hash_password(bytes(clear, 'utf-8'), b_salts)
    return compare_digest(bytes.fromhex(new_hash), b_hashed)

def user_connexion(mail: str, password:str) -> bool:
    """
    Docstring for user_connexion
    
    :param mail: Description
    :type mail: str
    :param password: Description
    :type password: str
    :return: Description
    :rtype: bool
    """
    user = Bd.get_user(mail)
    user_salt = Bd.get_salt(mail)
    return __compare_hash_clear(password, user_salt, user['password'])


def create_salt():
    """create a 16 bytes salt to hash password"""
    buffer = os.urandom(16)
    return buffer
