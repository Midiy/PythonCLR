parser grammar PythonParser;


options { tokenVocab=PythonLexer; }

program: (NewLine | classStatement)* ;


print: Print OpenParen expressionList CloseParen ;


classDef : Class Identifier (OpenParen identifierList CloseParen)? Colon classBlock ;

funcDef : Def Identifier OpenParen argList CloseParen (Arrow type)? Colon block ;

type : Identifier bracketedTypeList? ;

bracketedTypeList : OpenBracket (type | typeList) CloseBracket ;

typeList : (type | bracketedTypeList) Comma (type | bracketedTypeList)+ ;

classBlock : NewLine Indent classStatement+ Dedent ;

block : NewLine Indent statement+ Dedent ;

classStatement : statement | classDef ;

statement : onelineStatement | multilineStatement ;

onelineStatement 
  : ( print
      | assignStatement
      | flowStatement 
      | assertStatement
      | Pass
    )
    NewLine ;

assignStatement : singleAssignStatement | multipleAssignStatement ;

singleAssignStatement : (typedIdentifier | expression) Assign expression ;

multipleAssignStatement : (typedIdentifierList | expressionList) Assign expressionList ;

flowStatement 
  : returnStatement
  | raiseStatement
  | Break
  | Continue ;

returnStatement : Return expressionList ;

raiseStatement : Raise expression ;

assertStatement : Assert expression (Comma expression)? ;

multilineStatement 
  : funcDef
  | ifElifElseStatement
  | whileElseStatement
  | forElseStatement
  | tryStatement ;

ifElifElseStatement : ifStatement elifStatement* elseStatement? ;

ifStatement : If expression Colon block ;

elifStatement : Elif expression Colon block ;

elseStatement : Else Colon block ;

whileElseStatement : whileStatement elseStatement? ;

whileStatement : While expression Colon block ;

forElseStatement : forStatement elseStatement? ;

forStatement : For typedIdentifierList In expression Colon block;

tryExceptFinallyStatement : tryStatement exceptFinalyStatement ;

tryStatement : Try Colon block ;

exceptFinalyStatement 
  : (exceptStatement+ finalyStatement?)
  | finalyStatement ;

exceptStatement : Except (Identifier (As Identifier)?)? Colon block ;

finalyStatement : Finally Colon block ;

argList : argument (Comma argument)* ;

identifierList : Identifier (Comma Identifier)* ;

typedIdentifierList : typedIdentifier (Comma typedIdentifier)* ;

argument 
  : typedIdentifier 
  | defaultArgument ;

defaultArgument : typedIdentifier Assign expression ;

typedIdentifier : Identifier (Colon type)? ;

expressionList : expression (Comma expression)* ;

expression : orExpr ;

orExpr : andExpr (Or andExpr)* ;

andExpr : maybeNotExpr (And maybeNotExpr)* ;

maybeNotExpr : Not maybeNotExpr | compExpr ;

compExpr : bitOrExpr (compOperator bitOrExpr)* ;

compOperator 
  : Less
  | Greater
  | Equals
  | NotEquals
  | LessOrEquals
  | GreaterOrEquals ;

bitOrExpr : bitXorExpr (BitOr bitXorExpr)* ;

bitXorExpr : bitAndExpr (BitXor bitAndExpr)* ;

bitAndExpr : shiftExpr (BitAnd shiftExpr)* ;

shiftExpr : addOrSubExpr (shiftOperator addOrSubExpr)* ;

shiftOperator 
  : LeftShift
  | RightShift ;

addOrSubExpr : mulOrDivExpr (addOrSubOperator mulOrDivExpr)* ;

addOrSubOperator
  : Add
  | Sub ;

mulOrDivExpr : unaryOrPowExpr (mulOrDivOperator unaryOrPowExpr)* ;

mulOrDivOperator 
  : Mul
  | Div
  | Mod
  | Idiv ;

unaryOrPowExpr : unaryOperator unaryOrPowExpr | powExpr ;

unaryOperator 
  : Add
  | Sub
  | BitNot ;

powExpr : atomicExpr fieldRef* (Pow unaryOrPowExpr)* ;

fieldRef : Dot Identifier ;

atomicExpr 
  : Identifier
  | literal ;

literal 
  : StringLiteral 
  | NumberLiteral 
  | BoolLiteral ;
