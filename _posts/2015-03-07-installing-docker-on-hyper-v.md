---
layout: post
title: "Installing Docker on Hyper-V"
description: "Installing Docker on Windows"
author: "Devon Burriss"
category: Virtualization
tags: [Hyper-V, Ubuntu]
comments: true
permalink: installing-docker-on-hyper-v
excerpt_separator: <!--more-->
published: true
---

To be clear, currently Docker containers do not run on Windows. Microsoft is working with Docker to release something with feature parity but we will be lucky if we see that in 2015 ([Blogged by Scott Gu](http://weblogs.asp.net/scottgu/docker-and-microsoft-integrating-docker-with-windows-server-and-microsoft-azure)). So although there is a client for Windows for managing Docker containers, we will need an Ubuntu install. [Installing Ubuntu on Hyper-V](http://devonburriss.me/installing-ubuntu-on-hyper-v/)

![Docker logo](/img/posts/2015/large_h.png)
<!--more-->

# Installing Docker

Most of this is straight from the [Docker documentation](https://docs.docker.com/installation/ubuntulinux/) but I ran into a few problems that I think may be due to this running on Hyper-V. Also I wanted a quick reference in the future.

First lets update our package repositories:
`sudo apt-get update`

Currently the Docker docs mention pulling from their private repos to get the latest version but that was for Ubuntu 14.04. I noticed Ubuntu 14.10 repos contain Docker 1.2 which is at time of writing good enough for me.

So lets install Docker:
`sudo apt-get install docker.io`

Then so we get bash completion we can type:
`source /etc/bash_completion.d/docker.io`
No **sudo** needed. Alternatively just reboot with:
`sudo reboot`

Lets test our Docker install:
`sudo docker version`
`sudo docker info`

This displays version number of the components and some basic info on the install respectively.

The info will contain a line **WARNING: No swap limit support** so lets fix that.
`sudo nano /etc/default/grub`

Find the line **GRUB___CMDLINE___LINUX** and edit it:
`GRUB_CMDLINE_LINUX="cgroup_enable=memory swapaccount=1"` then save and exit nano.

We need to update Grub and reboot.
`sudo update-grub`

`sudo reboot`

Now running `sudo docker info` you will see the warning is gone.

If we try download and run a docker image we are still not there yet but lets try:
`sudo docker run -i -t ubuntu /bin/bash`

### Troubleshooting

#### Unexpected EOF
This actually happens every now and again with Docker (I think if latency is bad) so just try run the command again and it will likely work.

#### dial tcp: lookup registery-1.docker.io: no such host
The documentation explains how to add a dns to the docker options in **/etc/default/docker** but this actually didn't work for me on the Hyper-V. I had to edit **/etc/resolv.conf** and add the google nameserver there (doesn't have to be google).
`sudo nano /etc/resolv.conf`
Then add **nameserver 8.8.8.8** on a new line. Save and exit.
You might need to `sudo reboot`.

### Finally lets run something
So now we should be ready to go. Run
`sudo docker run -i -t ubuntu /bin/bash` again.
This should now pull down the ubuntu image and start up a container running ubuntu (yes we are running Ubuntu in a kernal process on another Ubuntu - inception right?).
The `-t` is to assign a terminal and `-i` is so the connection is interactive. 
Once it is running a terminal prompt will be available. Type `echo 'Hi'`. The Ubuntu container willl say hi back :)

So thats it. You have Docker running on a Hyper-V guest.

