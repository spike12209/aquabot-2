# Aquaforms
Aquaforms is an uninstrusive automated QA testing libray for windows forms. This
doesn't mean you are wonna get rid of the QA team but rather that those 
guys ain't gonna run the same test (manually) twice. It's all about that.


# How it works
The work is divided into two phases. The **recod phase** which capture 
the user input and create the testing script; and the **replay phase** that 
executes the testing script one step at a time. 
During the record phase the user can flag errors and add notes on the go.


## Record phase
Before start recording we must tell aqua which form to wacth. This is
an easy step. It goes something like this:

``` cpp
Aqua.Watch(targetForm);
```

Once you do that, aqua will look for changes on each and every control within
the form (and container controls, recursively). As of now it'll look for
value changes, show/hide changes and enable/disable changes. We may record more 
state changes in the future such as position, size and so on; but right now
we concentrate on the bare essentials.


### How we capture the user input
The user input is captured in frames. Every time the user changes a value
aqua creates a new frame to represent that particular change and 
its context (the state of every other control within the form). Those frames
are stored and can be compared to other frames during the replay phase to
detected differences between runs.


## Replay phase
As we mentioned above, aqua creates a script to capture user inputs and
saves the state of the whole form after each modification. Using that script
the library can replay the user interaction and compare the state of the
system after each step.


## Why this library is uninstrusive?
Is uninstrusive because you don't have to extend from so and so class, or
add attributes to your classes, or anything like that. The only thing you need
to do is to tell aqua which form to look at and the library takes it from there.

## Limitations
1. Once aqua started watching a form it assumes that all handlers have been
hooked. This means that if you attach an event handler to a control *after*
you called Aqua.Watch, the behaviour is undefined. So don't do that.

# How to build
On unix systems you can build the library by runing **make**.

TODO:
how to build on windows

# How to report bugs
TODO:
