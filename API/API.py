from flask import Flask, request, jsonify, Response
from datetime import datetime, timedelta, timezone
from Security.security import *
import jwt, json, bd

app = Flask(__name__)

SECRET_KEY = "a7w1dx58d41cdsdf51dfvsf8541sdfvsf3"

@app.route("/register", methods=["POST"])
def register():
    UserCredential = request.get_json(silent=True)
    print("Received registration request with data:", UserCredential)
    response = {}
    # Since the user is creating an account let's hash his password and
    # store the salt used
    # let's add the user's information in the database with our custom
    # method and return the database response with the token
    if bd.add_user(UserCredential['UserId'], UserCredential['Password'], 
                   UserCredential['Mail'], UserCredential['DisplayName']):
        # for now let's jsut accept anything and just send back the jwt token
        # let's create a dict with the important user info
        now = datetime.now(tz=timezone.utc)
        creation = int(now.timestamp())
        exp = int((now + timedelta(hours=1)).timestamp())
        print(f"creation time : {creation}, expiration time : {exp}")
        Payload = {"Role" : "user", "iss" : "API.me", "sub" : UserCredential.get("UserId"), "aud" : "chat-service", "iat" : creation, "exp" : exp}
        token = create_jwt_token(Payload)
        response = {"Token" : token}
        print("response:", response)
        return Response(json.dumps(response), status=200, mimetype='application/json')
    response = {"message" : "User creation refused"}
    return Response(json.dumps(response), status=200, mimetype="application/json")


if __name__ == "__main__":
    app.run("0.0.0.0",port=50000, threaded=True, debug=True)