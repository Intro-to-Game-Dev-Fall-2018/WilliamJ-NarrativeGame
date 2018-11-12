You open your messenger app.

There is an unread message in the group chat.

VAR impressionScore = 0

-> Msg1

= Msg1 

* {not Group2} [Check Group Chat] -> Group1
* {not Emily2} [Message Emily] -> Emily1
* {not Michael2} [Message Michael] -> Michael

=== Group1
Eugene: Anyone near the city this weekend?

+ [i think i am. why?] -> Group1A1
+ [...] -> Group1_1

=== Group1_1
Amy: I might be around with some friends on Saturday. Wanna meet up somewhere?

Eugene: yea! ill be at midtown at 12 so lmk.

+ [i think i'll be free too. can i come with?] -> Group1A2
+ [...] -> Group1_2

=== Group1_2
Michael: I think i'll be back in NY on Saturday too with my gf. Are we invited :)

Everyone else: H0LY S**T (yes);

+ [wait you're actually coming back this thanksgiving?] -> Group1_3
* {not Group1A1} {not Group1A2} [wait if Michael's here, i'll be make myself free Saturday then] -> Group1B1

=== Group1_3
Michael: yeah my parents asked me to introduce her and she agreed to come visit. 

The chat turns into a flurry of questions about his girlfriend Miranda.

+ [Return to messenger] -> Msg1

=== Group1B1
Amy: wow so you're only coming since Michael's coming? :/

+ [uh.....................] -> Group1B2
+ [ofc not. i love ALL my friends equally] -> Group1B2

=== Group1B2
Amy: ...i'll let you guys know where to meet on Saturday later.

+ [karaoke on Saturday anyone?] -> Group1B3
+ [<3] -> Msg1

=== Group1B3
Amy: ...
Michael: ...
Stephen: ...

Emily: wait if there's karaoke i'll come too

+ [see, Emily likes karaoke too! dont judge] -> Group1B4

=== Group1B4
Amy: fine, we'll see. see you guys Saturday

+ [Return to Messenger] -> Msg1

=== Group1A2
Eugene: yeah sure! the more the merrier!

+ [cool] -> Group1_3

=== Group1A1
Eugene: wanna hang? i'm gonna be in midtown for something but i'll have time after and i'll probably be bored
Amy: wait i'll be in the city too with some friends. we can go get lunch and do something after

+ [alright cool. lmk where we meet] -> Group1_2
