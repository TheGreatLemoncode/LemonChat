from API.Security.password import *

my_test = "hello world"
my_salt = "f3ef69b7249269e7f7a03f356c6b7caa"
my_hash = "f09266f26abae1c1cb4720b009c68616843dd185cd4db43272095d817ea9863d4c382bcfa2e93265a2fc8ef3bc45ec304be037273897221483f656772c861b1c"

pepper = os.urandom(32)
print(pepper.hex())
