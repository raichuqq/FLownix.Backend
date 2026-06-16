import os
from locust import HttpUser, task, between


class FlownixUser(HttpUser):
    wait_time = between(1, 3)

    access_token = os.getenv("FLOWNIX_ACCESS_TOKEN", "")

    def auth_headers(self):
        if not self.access_token:
            return {}

        return {
            "Authorization": f"Bearer {self.access_token}"
        }

    @task(4)
    def health(self):
        self.client.get("/health", name="GET /health")

    @task(2)
    def get_water_objects(self):
        self.client.get(
            "/api/WaterObjects",
            headers=self.auth_headers(),
            name="GET /api/WaterObjects",
        )

    @task(2)
    def get_pumps(self):
        self.client.get(
            "/api/Pumps",
            headers=self.auth_headers(),
            name="GET /api/Pumps",
        )

    @task(1)
    def get_alerts(self):
        self.client.get(
            "/api/Alerts",
            headers=self.auth_headers(),
            name="GET /api/Alerts",
        )