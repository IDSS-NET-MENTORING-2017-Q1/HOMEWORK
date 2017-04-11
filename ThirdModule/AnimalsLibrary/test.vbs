set dog = CreateObject("AnimalsLibrary.Dog")

WScript.Echo(dog.GetVoice())
WScript.Echo(dog.GetSatiety())
WScript.Echo(dog.GetHealth())

WScript.Sleep 10000

WScript.Echo(dog.GetSatiety())
WScript.Echo(dog.GetHealth())

dog.Feed()
dog.Heal()

WScript.Echo(dog.GetSatiety())
WScript.Echo(dog.GetHealth())