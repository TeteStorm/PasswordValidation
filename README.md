# # ValidatePasswordApi

#### APi para valida��o de senha seguindo as seguintes premissas:

- Tamanho m�nimo 9 caracteres
- Ao menos 1 d�gito
- Ao menos 1 letra min�scula
- Ao menos 1 letra mai�scula
- Ao menos 1 caractere especial do conjunto: !@#$%^&*()-+
- N�o possuir caracteres repetidos dentro do conjunto
- Tamanho m�ximo de 74 caracteres (pensando no "pior" caso 26 maiusculas + 26 mai�sculas + 10 d�gitos + 12 caracteres especiais todos sem repeti��o)

#### Projeto desenvolvido em .Net 5.0 
Componentes da aplica��o

## Projeto da Api MVC
#### PasswordValidationApi  
Para executar o projeto: 
- Ctrl+F5 acessar http://localhost:51696

##### Payload exemplo:
```
POST /registration HTTP/1.1
Host: localhost:51696
Content-Type: application/json
{
    "Password":"acbDEF1!@#$%^&*()+-"
}
```
## Projeto de Testes NUnit
#### PasswordValidationUnitTests
Para executar os testes projeto:
- Menu Test > Run All Tests ou Ctrl+R,A

--- 

#### Bibliotecas Utilizadas

### Fluent Validation 
A small validation library for .NET that uses a fluent interface and lambda expressions for building validation rules.
Allows you to define complex rules for object validation in a fluent way, making it easy to build and understand validation rules. You can find the project source on GitHub and read the documentation on their website.
https://github.com/FluentValidation/FluentValidation


### Password Generator
A .NET Standard library which generates random passwords with different settings to meet the OWASP requirements
https://github.com/prjseal/PasswordGenerator

---
## Descri��o da solu��o

Como estr�t�gia adotada para a solu��o visando robustez e flexibilidade foi adotado o uso da consagrada biblioteca de valida��o *Fluent Validation* que nos permite regras de valida��o complexas e customiz�veis, combinado � riqueza da engine de express�es regulares do .net.

A solu��o adotada contou com a cria��o de um validador customizado no qual tamb�m foram definidas os patterns com as regras a serem seguidas para compor as express�es regulares.

Para cada premissa as regras foram definidas individual e explicitamente visando a legibilidade e facilidade da manuten��o. Modifica��es nas regras e inclus�o de novas cl�usulas ficam facilitadas usando este approach.

A valida��o come�a tratando quando o campo n�o possui valor seguido da verifica��o de tamanho (adotado m�ximo de 74 caracteres n�o repetidos)
As regras de valida��o s�o tratadas via express�o regular uma por uma (com a estrat�gia de continuar avaliando ap�s quebrar uma regra para no final poder ter todos os erros)  regras restritivas, criticando caso a senha informada n�o tenha a premissa requerida.
No final uma express�o regular(aqui uso de classe est�tica para ser mais perfom�tica) avalia se a express�o atende combinadamente todas as regras.

Algumas escolhas adotadas pela solu��o:
- Expor as strings com os patterns foi vizando a testabilidade do mesmos.
- Uso de sobrecargas com TimeOut condicionando tempo limite diminuindo a influ�ncia de backtracking que possa afetar o desempenho (adotado conforme sugere a pr�pria lib evitando que algum tipo de input malicioso possa de alguma maneira travar a aplica��o).
- Tamb�m foi evitado o uso das chamadas Evil Regexes*.
- Uso do Cache das express�es regulares onde poss�vel (exceto .Matches() da api de fluent valitation por n�o disponibilidar a op��o de timeout), evitando degrada��o de performance.
- Testes exaustivos utilizando dados inv�lidos e quase v�lidos, bem como informa��es v�lidas.

```
* Evil Regex pattern contains:
Grouping with repetition
Inside the repeated group:
Repetition
Alternation with overlapping
Examples of Evil Patterns:

(a+)+
([a-zA-Z]+)*
(a|aa)+
(a|a?)+
(.*a){x} for x \> 10

```

#### Refer�ncias:
https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices

