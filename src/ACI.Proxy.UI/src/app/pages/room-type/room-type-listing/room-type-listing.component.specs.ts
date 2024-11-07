import { ComponentFixture, TestBed } from '@angular/core/testing';

import { RoomTypeListingComponent } from './room-type-listing.component';

describe('RoomTypeListingComponent', () => {
  let component: RoomTypeListingComponent;
  let fixture: ComponentFixture<RoomTypeListingComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [RoomTypeListingComponent]
    });
    fixture = TestBed.createComponent(RoomTypeListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});