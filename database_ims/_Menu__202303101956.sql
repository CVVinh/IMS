INSERT INTO public."Menu" ("idModule",title,icon,"view",controller,"action",parent,"isDeleted") VALUES
	 (1,'OTs','bx bx-time-five','ViewSubmenuVue','ots','click',1,0),
	 (2,'Users','bx bx-user','ViewMenuVue','users','click',0,0),
	 (2,'Users','bx bx-user','ViewSubmenuVue','users','click',5,0),
	 (2,'Groups','bx bxs-group','ViewSubmenuVue','groups','click',5,0),
	 (2,'Permission Group','bx bxs-book-reader','Submenu','permissionUserGroup','click',5,0),
	 (2,'Permission User','bx bxs-bookmark-alt-plus','Submenu','permissionUserMenu','click',5,0),
	 (3,'Groups','bx bxs-group','ViewMenuVue','groups','click',0,0),
	 (3,'Groups','bx bxs-group','ViewSubmenuVue','groups','click',11,0),
	 (3,'Users','bx bx-user','SubMenu','users','click',11,0),
	 (3,'Permission User','bx bxs-bookmark-alt-plus','SubMenu','permissionUserMenu','click',11,0);
INSERT INTO public."Menu" ("idModule",title,icon,"view",controller,"action",parent,"isDeleted") VALUES
	 (3,'Permission Group','bx bxs-book-reader','SubMenu','permissionUserGroup','click',11,0),
	 (6,'Equipment','bx bx-devices','ViewMenu','equipments','click',0,0),
	 (6,'Devices','bx bx-devices','SubMenu','devices','click',16,0),
	 (6,'Handover','bx bxs-hand','SubMenu','handover','click',16,0),
	 (6,'Orders','bx bxs-cart-add','SubMenu','listdevice','click',16,0),
	 (11,'Menu','bx bx-list-ul','ViewMenu','menu','click',0,0),
	 (11,'Menu','bx bx-list-ul','SubMenu','menu','click',20,0),
	 (11,'Modules','bx bxs-component','SubMenu','modules','click',20,0),
	 (10,'Modules','bx bxs-component','ViewMenu','modules','click',0,0),
	 (10,'Modules','bx bxs-component','SubMenu','modules','click',23,0);
INSERT INTO public."Menu" ("idModule",title,icon,"view",controller,"action",parent,"isDeleted") VALUES
	 (10,'Menu','bx bx-list-ul','SubMenu','menu','click',23,0),
	 (5,'OTs','bx bx-time-five','ViewMenu','ots','click',0,0),
	 (7,'Wikis','bx bx-note','ViewMenu','wikis','click',0,0),
	 (8,'Leave Off','bx bx-log-out-circle','ViewMenu','leaveOff','click',0,0),
	 (12,'Permission','bx bx-badge-check','ViewMenu','Permission/permissionUserMenu','click',0,0),
	 (12,'Permission Group','bx bxs-book-reader','SubMenu','permissionUserGroup','click',31,0),
	 (12,'Permission User','bx bxs-bookmark-alt-plus','SubMenu','permissionUserMenu','click',31,0),
	 (12,'Roles','bx bxs-lock','Submenu','roles','click',31,0),
	 (1,'Project','bx bx-notepad','projectView','project','click',0,0),
	 (1,'Project','bx bx-notepad','projectSub','project','click',1,0);
INSERT INTO public."Menu" ("idModule",title,icon,"view",controller,"action",parent,"isDeleted") VALUES
	 (8,'Add Task','bx bxs-layer-plus','ViewSubmenuVue','addtask','click',1,0);
