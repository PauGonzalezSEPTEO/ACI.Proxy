import { ComponentFixture, TestBed } from '@angular/core/testing';

import { IntegrationListingComponent } from './integration-listing.component';

describe('IntegrationListingComponent', () => {
  let component: IntegrationListingComponent;
  let fixture: ComponentFixture<IntegrationListingComponent>;

  beforeEach(() => {
    TestBed.configureTestingModule({
      declarations: [IntegrationListingComponent]
    });
    fixture = TestBed.createComponent(IntegrationListingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});