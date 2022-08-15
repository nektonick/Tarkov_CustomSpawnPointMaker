import { DependencyContainer } from "tsyringe";
import { DatabaseServer } from "@spt-aki/servers/DatabaseServer";
import { StaticRouterModService } from "@spt-aki/services/mod/staticRouter/StaticRouterModService";
import { HttpResponseUtil } from "@spt-aki/utils/HttpResponseUtil";

import { Logger } from "./Logger";
import config from "../config/config.json";
import { LocaleService } from "@spt-aki/services/LocaleService";

export class ClientHandler
{
    constructor(readonly container: DependencyContainer, readonly logger: Logger) {}

    load(): void
    {
        this.logger.debug("Loading ClientHandler...");

        const staticRouterModService = this.container.resolve<StaticRouterModService>("StaticRouterModService");
        const httpResponseUtil = this.container.resolve<HttpResponseUtil>("HttpResponseUtil");

        staticRouterModService.registerStaticRouter(
            "Lua-ScavMapKeyAccessPatcher-/Lua/ScavMapAccessKeyPatcher/config/",
            [
                {
                    url: "/Lua/ScavMapAccessKeyPatcher/config",
                    action: (url: string, info: any, sessionID: string, output: string): any => 
                    {
                        return httpResponseUtil.noBody(this.getMapConfig());
                    }
                }
            ],
            "Lua-ScavMapKeyAccessPatcher"
        );

        this.logger.debug("Completed ClientHandler Loading...");
    }

    public getMapConfig(): any
    {
        this.logger.info(this.container.resolve<LocaleService>("LocaleService").getDesiredLocale());
        const databaseServer = this.container.resolve<DatabaseServer>("DatabaseServer");
        const items = databaseServer.getTables().templates.items;

        const list = {};
        for (const map of Object.keys(config))
        {
            const keyItem = config[map]?.AccessKey || null;
            const item = items[keyItem] || null;

            if (!keyItem)
            {
                continue;
            }

            if (!item)
            {
                this.logger.error(`Map "${map}" has bad "AccessKey" item that doesn't exist "${keyItem}", skipping item...`);
                continue;
            }

            if (item?._type != "Item")
            {
                this.logger.error(`Map "${map}" has bad "AccessKey" key which is not an item "${keyItem}", skipping item...`);
                continue;
            }

            list[map] = keyItem;
        }
        this.logger.debug(`Sending config to client...\n${list}`);
        return list;
    }
}