<h2 align="center">
  <br>
  <img src="https://github.com/ivanmirandastavenuiter/victoria-tap-counter/blob/devel/assets/beer.jpg" alt="UFO" width="400">
  <br>
  <br>
  Victoria Tap Counter
  <br>
  <br>
  <img alt="owner" src="https://img.shields.io/badge/version-1.0-green" />
  <br>
  <br>
  
</h2>

### Introduction and structure
---
` [ 👷 ] `

#### **Layering**

<p style="text-align:justify">The application has been layed out as an <b>N-tier</b> application. Specifically, 3-tier application, which separates logically and conceptually the core parts of the applications, following code architecture and distribution <b>best practices by Robert C Martin and Gary McLean.</b></p>

- <b>Controllers</b>: api entry point
- <b>Services</b>: business logic
- <b>Repository</b>: repository pattern / data access layer

#### **SOLID**

<p style="text-align:justify">Another big remark on code distribution has been <b>strict following of SOLID principles</b>. From bottom to top, the application has been thought from the very
first moment <b>as loose coupled as possible</b> regarding dependencies interaction and tried to applied each of the principles one by one when constructing and delivering the code.</p>

- <p style="text-align:justify"><b>SRP or Single Responsibility Principle</b>: classes will have just one reason to change, thus 1 single responsibility. Any extra code or task to be accomplished has been properly isolated in separate classes.</p>
- <p style="text-align:justify"><b>Open/Closed principle</b>: A class should be potentially extended, but not changed in its previous behavior. Classes have been extended while maintaining initial behavior targets.</p>
- <p style="text-align:justify"><b>Liskov substitution</b>: An superclass can be replaced with a subclass at any time, then causing 0 collateral effect to the code response.</p>
- <p style="text-align:justify"><b>Interface segregation</b>: code has been abstracted through interface usage, thus allowing for loose relations among classes and dependencies. Interfaces are wide used into the app.</p>
- <p style="text-align:justify"><b>Dependency injection</b>: dependencies should be provided in a controlled, asbtracted, centered manner. Inversion of control has been applied for each of the classes in the application.</p>

#### **Design Patterns**

Design patterns are well known because of its utility. A few of them have been implemented in the aplication. 

- Template: for validator
- Adapter: usage of interfaces

<b>Potentially applicable</b>: builder in tests (organizational) and decorator for SRP.

### Assumptions
---
` [ 👇 ] `

The application follows the rules for the counter API.

1. First you need to create at least 1 dispenser.
2. You can change the status as long as the request is valid.
3. You can query the current status for the dispenser.

### Validations
---
` [ ✅ ] `

Input is validated with common and custom requirements. Those are:

- Not empty/null values
- Existence of resources referenced
- Valid values and formats
- Comply with business logic rules: status is not in conflict, dates are correct

### Run it 
---
` [ 🏃 ] `

Run it with Visual Studio or through CLI 

### Tests
---
` [ 🏭 ] `

<b>Testing is widely applied!</b> <b>Unit test</b> for main core logic has been implemented and also a full suite of <b>integration tests</b> to provide 100% coverage.

### Third party libraries
---
` [ 📑 ] `

- FluentAssertions
- FluentValidations
- AutoFixture
- Moq

### License
---
` [ ❗ ] `

MKNA Devs ©
