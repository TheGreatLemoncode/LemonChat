# 1- 2026-01-05 entry : Beginning of the project
Hello there, lemon speaking. I'm starting this chat app in advance because school is coming up again.
This chat app will be the second biggest project i have ever been part of. I really want to display 
knowledge of multiple language and my resourcefulness. I will make use of no AI except to ask last 
minute questions about design. Wish me luck.


# 2- 2026-01-08 : First bug, Full deadlock
Originally, i was going to code straight non-stop the easy stuff like the bones of the backend and a bit of UI. After finishing, those i wanted to try the connector by simula
ting an authentification to my api. During the debug, i noticed that the request would
go to the Api but the whole client side would freeze. I spent the whole day looking into 
it and you know what i found ? Apparently it's a common situation in wpf and .net called 
a Deadlock. It happen when you call an async method in a synch one and do exacly this:
asyncmethod().result . It freeze the whole UI, stop exceptions from happening and cut 
communication. The solution is to change the result and also that anywhere you call an 
async method must be async also. That's all for this entry. From you friend the lemon.

# 3- 2026-01-10 : A bit of side quests
So i was making the code on the server side and got a sudden idea: "what if all those long
lines get turn in functions ?". So i start to code some of them and without realising it i
was creating a full on library to handle token creation and other security thing. It might
take a bit more time in my planning but i have nothing better to do. End of line, this 
project will include a python library. Yeah i know, what am i doing ?

# 4- 2026-01-13 : Creation of a new set of communication codes
Some time ago, I thought of a problem: “What if the server doesn’t send back the token during 
the authentication step, but instead sends a message indicating whether the user is already in
the database?” (yeah, I overthink a lot).So I decided to completely change how the server 
communicates. Now, the server only sends back Content and a Code that tells the client how it
should behave. To do this, I had to change the client so that it never knows what’s coming, 
but knows there will always be a code to guide it. It only took me three days of straight 
bed-rotting and doom-scrolling to reach this conclusion lol.