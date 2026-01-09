from flask import Flask, request, jsonify, Response
from datetime import datetime, timedelta, timezone
import jwt, json

app = Flask(__name__)

SECRET_KEY = "a7w1dx58d41cdsdf51dfvsf8541sdfvsf3"

@app.route("/register", methods=["POST"])
def register():
    UserCredential = request.get_json(silent=True)
    print("Received registration request with data:", UserCredential)
    # for now let's jsut accept anything and just send back the jwt token
    # let's create a dict with the important user info
    now = datetime.now(tz=timezone.utc)
    creation = int(now.timestamp())
    exp = int((now + timedelta(hours=1)).timestamp())
    print(f"creation time : {creation}, expiration time : {exp}")
    Payload = {"Role" : "user", "iss" : "API.me", "sub" : UserCredential.get("UserId"), "aud" : "chat-service", "iat" : creation, "exp" : exp}
    token = jwt.encode(Payload,SECRET_KEY, algorithm="HS256")
    response = {"Token" : token}
    print("response:", response)
    return Response(json.dumps(response), status=200, mimetype='application/json')

if __name__ == "__main__":
    app.run("0.0.0.0",port=50000, threaded=True, debug=True)