import { StatsPerWishlist, ScreeningItem, User, Wishlist, ItemType } from './model';

export enum MenuItemTestExpert {
  todo = 0,
  archive = 1
}

export class Deadline {
  public date: Date;
  public count: number;
}

export class DashboardData extends StatsPerWishlist {
  public wishlist: Wishlist;
  public deadlines: Deadline[];
  public screeningsList: ScreeningItem[];
  public users: User[];
}
export class ProgressGraphData {
  public date: Date;
  public itemCountDeadlineEnd: number;
  public itemCountDeadlineExpected: number;
  public itemCountActual: number;
}

export class MenuItemManager {
  public label: string;
  public icon: string;
  public command: any;
}

export class WishlistSelectButton {
  public label: string;
  public value: string;
  public created: Date;
}

export class TestExportListItem {
  public id: string;
  public uniqueCode: string;
  public name: string;
  public learningobjective: string;
  public state: string;
  public deadline: string;
  public author: string;
  public lastModified: string;
  public latestScreeningId: string;
}

export class ManagerWishlistItem {
  public id: string;
  public learningobjective: string;
  public learningobjectiveCode: string;
  public deadline: string;
  public totalCount: number;
  public doneCount: number;
  public constructionCount: number;
  public screeningCount: number;
  public rejectedCount: number;
  public learningObjectiveId: string;
  public wishlistId: string;
  public itemsCreated: boolean;
}

export class ProgressBarData {
  public label: string;
  public value: number;
  public percentage: number;
  public type: string;
}

export class ItemTypeToChoose {
  public type: ItemType;
  public icon: string;
  public textRef: string;
}

export enum TipsType {
  do = 0,
  dont = 1
}

export class CollapsableHeader {
  name: string;
  collapsed: boolean;
}
