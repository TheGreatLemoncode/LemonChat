from API.Security.security import *

my_test = "hello world"
my_salt = "f3ef69b7249269e7f7a03f356c6b7caa"
my_hash = "f09266f26abae1c1cb4720b009c68616843dd185cd4db43272095d817ea9863d4c382bcfa2e93265a2fc8ef3bc45ec304be037273897221483f656772c861b1c"

hash = hash_password(my_test.encode("utf-8"), my_salt.encode("utf-8"))

print(f"same hello : {hash}")

print(f"compare with old value : {compare_hash_clear(my_test, my_salt, my_hash)}")
