﻿import { AppComponent } from "../Components/app.component";
import { ActiveTweets } from "../Components/activetweets.component";
import { TestBed, ComponentFixture } from "@angular/core/testing";
import { Tweet } from "../Models/tweet";
import { TweetStreamMock } from "../Services/tweetstream.service.mock";
import { TweetStream } from "../Services/tweetstream.service";

let component: ActiveTweets;
let fixture: ComponentFixture<ActiveTweets>;

describe("Admin panel active tweets component", () => {
    beforeEach(() => {
        TestBed.configureTestingModule({
            declarations: [ActiveTweets],
            providers: [{provide: TweetStream, useClass: TweetStreamMock}]
        });

        fixture = TestBed.createComponent(ActiveTweets);
        component = fixture.componentInstance;
    });

    it("Add element", () => {
        component.addTweet(new Tweet(1, 1, "", "", new Date(), "", "", [], []));
        fixture.detectChanges();
        expect(component.activeTweets.length).toEqual(1);
    });

    it("Remove element", () => {
        component.addTweet(new Tweet(0, 0, "", "", new Date(), "", "", [], []));
        component.removeTweet(0);
        fixture.detectChanges();
        expect(component.activeTweets.length).toEqual(0);
    });
});
