lexer grammar PythonLexer;


tokens { Indent, Dedent }

Skip : ( Spaces | Comment ) -> skip ;


// Keywords

True : 'True' ;
False : 'False' ;

And : 'and' ;
Or : 'or' ;
Not : 'not' ;

In : 'in' ;
As : 'as' ;

If : 'if' ;
Elif : 'elif' ;
Else : 'else' ;

For : 'for' ;
While : 'while' ;

Break : 'break' ;
Continue : 'continue' ;

Try : 'try' ;
Except : 'except' ;
Finally : 'finally' ;

Assert : 'assert' ;
Raise : 'raise' ;

Class : 'class' ;
Def : 'def' ;
Pass : 'pass' ;
Return : 'return' ;

None : 'None' ;


// Operators

Add : '+' ;
Sub : '-' ;
Mul : '*' ;
Div : '/' ;
Mod : '%' ;
Idiv : '//' ;
Pow : '**' ;

BitAnd : '&' ;
BitOr : '|' ;
BitXor : '^' ;
BitNot : '~';
LeftShift : '<<' ;
RightShift : '>>' ;

Assign : '=' ;

AddAssign : '+=' ;
SubAssign : '-=' ;
MultAssign : '*=' ;
DivAssign : '/=' ;
ModAssign : '%=' ;
IdivAssign : '//=' ;
PowAssign : '**=' ;
AndAssign : '&=' ;
OrAssign : '|=' ;
XorAssign : '^=' ;
LeftShiftAssign : '<<=' ;
RightShiftAssign : '>>=' ;

Less : '<' ;
Greater : '>' ;
Equals : '==' ;
NotEquals : '!=' ;
LessOrEquals : '<=' ;
GreaterOrEquals : '>=' ;

Dot : '.';
Comma : ',';
Colon : ':';
OpenParen : '(' ;
CloseParen : ')' ;
OpenBracket : '[' ;
CloseBracket : ']' ;

Arrow : '->' ;



// Literals

StringLiteral
 : '\'' ( StringEscape | ~[\\\n\r\f'] )* '\''
 | '"' ( StringEscape | ~[\\\n\r\f"] )* '"'
 | '\'\'\'' ( StringEscape | ~'\\' )* '\'\'\''
 | '"""' ( StringEscape | ~'\\' )* '"""'
 ;

IntLiteral : SignedIntNumber ;

FloatLiteral 
 : DotFloatLiteral 
 | ExpFloatLiteral
 ;

NumberLiteral 
 : IntLiteral
 | FloatLiteral
 ;

BoolLiteral : True | False ;


// General constructions

Print : 'print' ;

Identifier : LetterOrUnderscore (LetterOrUnderscore | Digit)* ;

NewLine
 : (Spaces
     | ( '\r'? '\n' | '\r' | '\f' ) Spaces?
   )
 ;


// Fragments

fragment Spaces : [ \t]+ ;

fragment Comment : '#' ~[\r\n\f]* ;

fragment StringEscape : '\\' . ;

fragment LatinLetter : [A-Za-z] ;

fragment LetterOrUnderscore : (LatinLetter | '_') ;

fragment Digit : [0-9] ;

fragment IntNumber : Digit+ ;

fragment NumberSign : [+-]? ;

fragment SignedIntNumber : NumberSign IntNumber;

fragment SignedFloatNumber
 : SignedIntNumber '.' IntNumber?
 | SignedIntNumber? '.' IntNumber
 ;

fragment DotFloatLiteral : SignedFloatNumber ;

fragment ExpFloatLiteral : (SignedIntNumber | SignedFloatNumber) [eE] SignedIntNumber ;