# fantasy-football-crawler

So this is a simple C# comand-line project, that parses most-used players (to a postges database) for a fantasy-football game on sports.ru .

This program runs once an hour on a Ubuntu server using Mono. The scheduling is done using Crontab.

TODO:
- [ ] Monogodb support, for easy node.js backend development.
- [ ] Multithreading support, if it will make my app work reasonably faster
- [ ] A system of command line arguments, to run my app on different databases, single/multithreaded ect.
