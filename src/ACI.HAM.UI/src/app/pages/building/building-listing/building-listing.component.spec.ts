import { ComponentFixture, TestBed } from '@angular/core/testing';

import { BuildingListingComponent } from './building-listing.component';

describe('BuildingListingComponent', () => {
  let component: BuildingListingComponent;
  let fixture: ComponentFixture<BuildingListingComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [BuildingListingComponent]
    });
    fixture = TestBed.createComponent(BuildingListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});