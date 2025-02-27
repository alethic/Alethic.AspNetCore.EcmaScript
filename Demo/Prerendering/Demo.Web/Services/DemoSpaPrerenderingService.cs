using Alethic.AspNetCore.EcmaScript.SpaServices.Prerendering.Services;
using Alethic.AspNetCore.EcmaScript.SpaServices.Routing;

using Demo.Data.Dal.Services;
using Demo.Web.Extensions;

namespace Demo.Web.Services;

public class DemoSpaPrerenderingService : ISpaPrerenderingService
{

	private readonly ISpaRouteService spaRouteService;
	private readonly IPersonService personService;

	public DemoSpaPrerenderingService(ISpaRouteService spaRouteService, IPersonService personService)
	{
		this.spaRouteService = spaRouteService;
		this.personService = personService;
	}

	public Task BuildRoutes(ISpaRouteBuilder routeBuilder)
	{
		routeBuilder
		   .Route("", "home")
		   .Group("person", "person", person_routes => person_routes
			   .Route("", "list")
			   .Route("create", "create")
			   .Route("{personid}", "show")
			   .Route("{personid}/edit", "edit")
			   .Route("{personid}/{name}", "show-name")
			   .Route("{personid}/{name}/edit", "edit-name")
		   );

		return Task.CompletedTask;
	}

	public async Task OnSupplyData(HttpContext context, IDictionary<string, object> data)
	{
		var route = await spaRouteService.GetCurrentRoute(context);
		switch (route?.Name)
		{
			case "home":
				await spaRouteService.Redirect(context, "person-list", new Dictionary<string, object> { });
				break;
			case "person-list":
				{
					var people = await personService.GetPeople();
					data["people"] = people;
				}
				break;
			case "person-show":
			case "person-edit":
				{
					var personid = Convert.ToInt32(route.Parameters["personid"]);
					var person = await personService.GetPerson(personid, false);
					if (person == null)
					{
						context.Response.OnStarting(() =>
						{
							context.Response.StatusCode = StatusCodes.Status404NotFound;
							return Task.CompletedTask;
						});
					}
					else
					{
						await spaRouteService.Redirect(context, $"{route.Name}-name", new { personid = personid, name = (person.FirstName + " " + person.LastName).Slugify() });
					}
				}
				break;
			case "person-show-name":
			case "person-edit-name":
				{
					var personid = Convert.ToInt32(route.Parameters["personid"]);
					var person = await personService.GetPerson(personid);
					if (person == null)
					{
						context.Response.OnStarting(() =>
						{
							context.Response.StatusCode = StatusCodes.Status404NotFound;
							return Task.CompletedTask;
						});
					}
					else if (route.Parameters["name"] == (person.FirstName + " " + person.LastName).Slugify())
					{
						data["person"] = person;
					}
					else
					{
						await spaRouteService.Redirect(context, route.Name, new { personid = personid, name = (person.FirstName + " " + person.LastName).Slugify() });
					}
				}
				break;
		}

		data.Add("message", "Message from server");
	}
}
