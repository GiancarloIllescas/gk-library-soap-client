
# GK.Library.Soap.Client

NuGet para **simplificar y estandarizar** el consumo de servicios SOAP (WCF) en .NET 8 con enfoque de arquitectura hexagonal.

---

## ¿Qué incluye?

* **Application / Ports (Out)**
  * `ISoapClientService` – puerto de salida desacoplado para crear clientes SOAP.
  * `SoapClientOptions` – DTO de configuración (endpoint, timeouts, headers, retries).

* **Infrastructure / Adapters**
  * `SoapClientService` – implementación resiliente que genera `ClientBase<T>` con `BasicHttpBinding`.
  * `PollySoapProxy` *(internal)* – intercepta llamadas y aplica políticas Polly (retry + back‑off).
  * `SOAPMessageLoggingBehaviour` *(internal)* – loguea XML request/response y ofusca datos sensibles.
  * `SoapClientHandle` – wrapper que cierra/aborta el canal WCF al finalizar el scope.

* **Infrastructure / Extensions**
  * `AddSoapClientService<TClient,TChannel>()` – extension method de DI para registrar un servicio SOAP con una sola línea.

---

## Instalación

```bash
# Se debe tener mapeado el feed donde se encuentra el nuget
dotnet add package GK.Library.Soap.Client
# o agregar manualmente desde Visual Studio
```

---

## 🚀 Uso

1. **Registro del servicio** en `Program.cs`:

```csharp

var builder = WebApplication.CreateBuilder(args);

// Registramos el MonolitoTransaccional
builder.Services.AddScoped<IMonolitoTransaccional, MonolitoTransaccional>();

var app = builder.Build();
   
app.Run();

```

2. **Consumirlo**:

```csharp

public class ClaseX
{
    private readonly IMonolitoTransaccional _monolito;

    public ClaseX(IMonolitoTransaccional monolito)
        => _monolito = monolito;

    public async Task EjecutarProcesoAsync(TransaccionRequest request, CancellationToken ct = default)
    {
        var resultado = await _monolito.EjecutarAsync(request, ct);
        if (!resultado.Exito)
        {
            // manejar error...
            throw new InvalidOperationException(resultado.Mensaje);
        }

        // continuar con lógica tras la transacción exitosa…
    }
}

```

---

## Opciones disponibles (`SoapClientOptions`)

| Propiedad                           | Descripción                                                  | Default |
|------------------------------------|--------------------------------------------------------------|---------|
| `Endpoint`                         | URL del `.svc` o endpoint SOAP                               | **obligatorio** |
| `Binding`                          | Binding custom (si se omite se genera uno HTTP/HTTPS)        | `BasicHttpBinding` |
| `TimeOutSeconds`                   | Timeout para open/close/send/receive                         | 30 s |
| `MaxBufferSize` / `MaxReceivedMessageSize` | Tamaño máx. de mensajes                             | 10 MB |
| `CustomHeaders`                    | Diccionario clave‑valor que se enviará en cada request       | `null` |
| `PollySettings.RetryCount`         | Intentos de reintento                                        | 3 |
| `PollySettings.WaitAndRetrySeconds`| Segundos de espera entre reintentos                          | 4 s |

---
