"""
Connexion à la BD
"""

import types
import contextlib
import mysql.connector


__ALL__ = [
    "add_user",
    "get_user",
    "get_salt"
]

@contextlib.contextmanager
def creer_connexion():
    """Pour créer une connexion à la BD"""
    conn = mysql.connector.connect(
        user="root",
        password="", # verifier ca toujours
        host="127.0.0.1",
        database="LemonChat",
        raise_on_warnings=True
    )

    # Pour ajouter la méthode getCurseur() à l'objet connexion
    conn.get_curseur = types.MethodType(get_curseur, conn)

    try:
        yield conn
    except Exception:
        conn.rollback()
        raise
    else:
        conn.commit()
    finally:
        conn.close()


@contextlib.contextmanager
def get_curseur(self):
    """Permet d'avoir *tous* les enregistrements dans un dictionnaire"""
    curseur = self.cursor(dictionary=True, buffered=True)
    try:
        yield curseur
    finally:
        curseur.close()


def add_user(password_hash, mail, name, salt : str):
    """Ajoute un utilisateur à la BD"""
    with creer_connexion() as conn:
        with conn.get_curseur() as curseur:
            sql = "INSERT INTO user (mail, name, password) VALUES (%s, %s, %s)"
            curseur.execute(sql, (mail, name, password_hash))
            print(f"User {mail} added to the database.")
            b_user= bool(curseur.rowcount > 0)
    b_salt = __add_salt_to_user(mail, salt)
    return bool(b_user and b_salt)

def __add_salt_to_user(user_mail : str, user_salt : str) -> bool:
    with creer_connexion() as conn:
        with conn.get_curseur() as curseur:
            sql = "INSERT INTO salt VALUES (null, (SELECT mail FROM user WHERE user.mail = %s), %s)"
            curseur.execute(sql, (user_mail, user_salt))
            print(f"salt of user {user_mail} added to the database.")
            return bool(curseur.rowcount > 0)

def get_user(user_mail: str):
    """Return the user information in the database with the given mail"""
    with creer_connexion() as conn:
        with conn.get_curseur() as curseur:
            sql = "SELECT * FROM user WHERE user.mail = %s"
            curseur.execute(sql, (user_mail,))
            return curseur.fetchone()

def get_salt(user_mail: str) -> str:
    """Return the salt of the user with the given mail"""
    with creer_connexion() as conn:
        with conn.get_curseur() as curseur:
            sql = "SELECT salt.value FROM salt WHERE salt.mail " \
            "= (SELECT mail FROM user where user.mail = %(mail)s)"
            curseur.execute(sql,{"mail" : user_mail})
            data = curseur.fetchone()
            return data['value']
