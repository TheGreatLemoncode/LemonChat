import json
from flask import Flask, request, Response
from datetime import datetime, timedelta, timezone
import Security
import Bd



app = Flask(__name__)

SECRET_KEY = "a7w1dx58d41cdsdf51dfvsf8541sdfvsf3"

@app.route("/register", methods=["POST"])
def register():

    """Flask endpoint that allow a user to create an account by
    sending his credential using the POST method"""

    usercredential = request.get_json(silent=True)
    print("Received registration request with data:", usercredential)
    response = {"Content" : "", "Code": 0}
    # Since the user is creating an account let's hash his password and
    # store the salt used

    # Let's use all our custum library to ease this endpoint
    # first we try to add the account to our database after hashing the password
    # and stocking the salt used
    salt = Security.create_salt()
    hashed = Security.hash_password(usercredential['Password'].encode("utf-8"), salt)
    if Bd.add_user(hashed, usercredential['Mail'], usercredential['DisplayName'] , salt.hex()):
        # if it is successfull we create a token with the information we have
        # about the user
        now = datetime.now(tz=timezone.utc)
        creation = int(now.timestamp())
        exp = int( (now + timedelta(hours=5)) .timestamp())
        # we initialize a payload to be turn into a token
        payload = {"Role" : "user", "iss" : "API.me", "sub" :
                   usercredential.get("Mail"), "aud" : "chat-service",
                   "iat" : creation, "exp" : exp}
        token = Security.create_jwt_token(payload)
        # with our token created we can create a response for the user
        response["Content"] = usercredential['DisplayName']
        response["Code"] = 30
        return Response(json.dumps(response), status=200,
                        mimetype="application/json", headers={'tk': token})

    # In the case that the user is already in our database, we return
    # just a message to inform him to use the login option
    response["Content"]= "Account already exists"
    response["Code"] = 35
    return Response(json.dumps(response), status=200, mimetype="application/json")

@app.route("/connexion", methods=["POST"])
def connexion():
    """
    Docstring for connexion
    """
    data = request.get_json(silent=True)
    user_mail = data['Mail']
    user_pswd = data['Password']
    if Security.user_connexion(user_mail, user_pswd):
        now = datetime.now(tz=timezone.utc)
        creation = int(now.timestamp())
        exp = int( (now + timedelta(hours=5)) .timestamp())
        # we initialize a payload to be turn into a token
        payload = {"Role" : "user", "iss" : "API.me", "sub" :
                   user_mail, "aud" : "chat-service",
                   "iat" : creation, "exp" : exp}
        token = Security.create_jwt_token(payload)
        # we get the user in the database
        user = Bd.get_user(user_mail)
        response = {"Content" : user['mail'], "Code" : 30}
        return Response(json.dumps(response), 200,
                        mimetype="application/json", headers={'tk' : token})
    response = {"Content" : "Connexion refused", "Code" : 35}
    return Response(json.dumps(response), 200, mimetype="application/json")


if __name__ == "__main__":
    app.run("0.0.0.0",port=50000, threaded=True, debug=True)
