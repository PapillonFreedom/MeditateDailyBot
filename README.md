//@MeditateDailyBot - Bot for daily meditation now in Telegram!
TODO:

Resurrect legacy code to actual state:

   1. [v]delete message "just play audio below"
   
   2. try to inspect how to calls files via telegram file id

Add custom minutes by user via sox-implementation on backend side:
      It should check sound directory for file existing:
        if true, then send this
        if false, then generate, name it and then send
        let pool of variable sounds will be of 1 - 60 (180) minutes
         place all realisation in other branch

[v]Describe HowToUse by command /help in Bot

[v]Rebuild all sound files: now they store locally on server, calls not by github link (because audion doesnt play correct on some devices)

[v]Aborted (core dumped) when using server sound files

[v]Test for stable work

Add gif for /help describing

[v]Deploy to AWS

Make docker-container with ready-to-deploy project

k8s?

Ansible?

Terraform?
