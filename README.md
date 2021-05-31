## ValidatePasswordApi

#### API para validação de senha seguindo as seguintes premissas:

- Tamanho mínimo 9 caracteres
- Ao menos 1 dígito
- Ao menos 1 letra minúscula
- Ao menos 1 letra maiúscula
- Ao menos 1 caractere especial do conjunto: !@#$%^&*()-+
- Não possuir caracteres repetidos dentro do conjunto
- Tamanho máximo de 74 caracteres (pensando no "pior" caso 26 maiusculas + 26 maiúsculas + 10 dígitos + 12 caracteres especiais todos sem repetição)

#### Projeto desenvolvido em .Net 5.0 
> Componentes da aplicação

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

 # Descrição da solução

Como estrátégia adotada para a solução visando robustez e flexibilidade foi adotado o uso da consagrada biblioteca de validação *Fluent Validation* que nos permite regras de validação complexas e customizáveis, combinado à riqueza da engine de expressões regulares do .net.

A solução adotada contou com a criação de um validador customizado no qual também foram definidas os patterns com as regras a serem seguidas para compor as expressões regulares.

Para cada premissa as regras foram definidas individual e explicitamente visando a legibilidade e facilidade da manutenção. Modificações nas regras e inclusão de novas cláusulas ficam facilitadas usando este approach.

A validação começa tratando quando o campo não possui valor seguido da verificação de tamanho (adotado máximo de 74 caracteres não repetidos)
As regras de validação são tratadas via expressão regular uma por uma (com a estratégia de continuar avaliando após quebrar uma regra para no final poder ter todos os erros)  regras restritivas, criticando caso a senha informada não tenha a premissa requerida.
No final uma expressão regular(aqui uso de classe estática para ser mais perfomática) avalia se a expressão atende combinadamente todas as regras.

Algumas escolhas adotadas pela solução:
- Expor as strings com os patterns foi vizando a testabilidade do mesmos.
- Uso de sobrecargas com TimeOut condicionando tempo limite diminuindo a influência de backtracking que possa afetar o desempenho (adotado conforme sugere a própria lib evitando que algum tipo de input malicioso possa de alguma maneira travar a aplicação).
- Também foi evitado o uso das chamadas Evil Regexes*.
- Uso do Cache das expressões regulares onde possível (exceto .Matches() da api de fluent valitation por não disponibilidar a opção de timeout), evitando degradação de performance.
- Testes exaustivos utilizando dados inválidos e quase válidos, bem como informações válidas.

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

#### Referências:
https://docs.microsoft.com/en-us/dotnet/standard/base-types/best-practices

