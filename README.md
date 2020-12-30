# GravyLang
A spare time project where I attempt to create a programming language

## Todo:
### Lexer (single line)
  1. Handle numbers with decimal places (directly touching words)
  
     e.g. some6.7teen -> some6, ., 7teen.
  3. add more delimiters and coverage
  4. add test coverage for new lexer
  
### Tokenizer:
  1. Extend functionality to support more types
  2. Add functionality to replace certain symbols with specifics e.g. = -> EQUAL rather than (Literal, =)
  
### LALR Table Generator:
  1. Rule Input (JSON/YAML/Custom)
  2. Generate Item sets /w Look aheads
  3. Generate translation table, see: [Video on creating translation table](https://www.youtube.com/watch?v=DYnyOeEXWuU)
  4. (Potentially optional Step) Generate Extended Grammar / FIRST/FOLLOW sets, see: [Article on creating LALR generator](https://web.cs.dal.ca/~sjackson/lalr1.html)
  
### Syntax Parser:
  1. Utilising LALR Table generated in previous step, parse tokens
